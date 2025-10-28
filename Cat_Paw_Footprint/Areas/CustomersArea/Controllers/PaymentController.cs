using Cat_Paw_Footprint.Areas.Order.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
    [Area("CustomersArea")]
    [Route("CustomersArea/[controller]")]
    [Authorize(AuthenticationSchemes = "CustomerAuth")]
    public class PaymentController : Controller
	{
		private readonly webtravel2Context _db;
		private readonly ECPayOptions _opt;
        private readonly ICustomerLevelService _levelSvc;
        private readonly IEmailSender _sender;
        private int CurrentCustomerId =>
        int.TryParse(User.FindFirst("CustomerId")?.Value, out var id) ? id : 0;
        private string PayItemsKey(int cid) => $"PAY_ITEMS_{cid}";
        private string CartKey => $"CART_ITEMS_{CurrentCustomerId}";
        public class ECPayOptions
		{
			public bool IsStage { get; set; } = true;
			public string MerchantID { get; set; } = "";
			public string HashKey { get; set; } = "";
			public string HashIV { get; set; } = "";
			public string ReturnURL { get; set; } = "";       // 後端回呼
			public string OrderResultURL { get; set; } = "";  // 前端導回（付款完成）
			public string ClientBackURL { get; set; } = "";   // 取消時返回
		}
		public PaymentController(webtravel2Context db, IEmailSender sender, IOptions<ECPayOptions> opt , ICustomerLevelService levelSvc)
		{
			_db = db;
			_sender = sender;
			_opt = opt.Value;
            _levelSvc = levelSvc;
        }
		// /CustomersArea/Payment
		[HttpGet("")]              // GET /CustomersArea/Payment
		public IActionResult Index()
				=> View("MockPay");    // 明確指向 Views/Payment/MockPay.cshtml

		[HttpGet("MockPay")]       // GET /CustomersArea/Payment/MockPay
		public IActionResult MockPay()
			=> View();             //        [HttpPost("PayMock")]

        [HttpPost("PayMock")]
        public async Task<IActionResult> PayMock(
        [FromForm] string name, [FromForm] string c1, [FromForm] string c2,
        [FromForm] string c3, [FromForm] string c4,
        [FromForm] string expRaw, [FromForm] string cvv)
        {
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();

            // 最基本的後端檢核（保留）
            if (string.IsNullOrWhiteSpace(name) ||
                c1?.Length != 4 || c2?.Length != 4 || c3?.Length != 4 || c4?.Length != 4 ||
                expRaw?.Length != 4 || (cvv?.Length != 3 && cvv?.Length != 4))
            {
                TempData["PayError"] = "資料不完整，請重新輸入。";
                return RedirectToAction("CheckoutCredit"); // 或回 MockPay
            }

            // ---------------- A) 從「訂單頁」來（單筆訂單付款） ----------------
            var orderIdStr = HttpContext.Session.GetString("PAY_ORDER_ID");
            if (!string.IsNullOrWhiteSpace(orderIdStr) && int.TryParse(orderIdStr, out var orderId))
            {
                var o = await _db.CustomerOrders.FirstOrDefaultAsync(x => x.OrderID == orderId && x.CustomerID == cid);
                if (o == null) return NotFound("訂單不存在");
                if (o.OrderStatusID != 2 /*未付款*/) return BadRequest("此訂單不可付款");

                o.OrderStatusID = 1; // 已付款
                o.UpdateTime = DateTime.Now;
                await _db.SaveChangesAsync();

                HttpContext.Session.Remove("PAY_ORDER_ID");

                // ★★ 標記本次使用的優惠券（若有）
                await MarkUsedCouponAsync(cid);

                // ★ 重算會員等級
                await _levelSvc.RecalculateAndUpdateAsync(cid);

                TempData["PayOk"] = "付款成功！";
                return Redirect("/CustomersArea/Orders");
            }

            // ---------------- B) 從「購物車頁」來（勾選多筆建立訂單） ----------------
            var idsStr = HttpContext.Session.GetString(PayItemsKey(cid)) ?? "";
            var idList = idsStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => int.TryParse(s, out var i) ? i : 0)
                               .Where(i => i > 0)
                               .ToHashSet();
            if (idList.Count == 0) return BadRequest("找不到要付款的商品");

            var cartJson = HttpContext.Session.GetString(CartKey);
            var items = string.IsNullOrWhiteSpace(cartJson)
                ? new List<CartItem>()
                : System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new();

            var payItems = items.Where(x => idList.Contains(x.ProductId)).ToList();
            if (payItems.Count == 0) return BadRequest("購物車中已無選取商品");

            var now = DateTime.Now;
            foreach (var it in payItems)
            {
                _db.CustomerOrders.Add(new CustomerOrders
                {
                    CustomerID = cid,
                    ProductID = it.ProductId,
                    OrderStatusID = 1, // 已付款
                    TotalAmount = it.Price * it.Qty,
                    CreateTime = now,
                    UpdateTime = now
                });
            }
            await _db.SaveChangesAsync();

            // 從購物車移除已付款項目
            items.RemoveAll(x => idList.Contains(x.ProductId));
            HttpContext.Session.SetString(CartKey, System.Text.Json.JsonSerializer.Serialize(items)); // ★ 寫回每用戶購物車
            HttpContext.Session.Remove(PayItemsKey(cid));

            // ★★ 標記本次使用的優惠券（若有）
            await MarkUsedCouponAsync(cid);

            // ★ 重算會員等級
            await _levelSvc.RecalculateAndUpdateAsync(cid);

            TempData["PayOk"] = "付款成功！已建立訂單。";
            return Redirect("/CustomersArea/Orders");
        }

        // ====== 新增：共用方法—把 Session 中的券標記為已使用 ======
        private async Task MarkUsedCouponAsync(int customerId)
        {
            var couponJson = HttpContext.Session.GetString("CART_COUPON");
            if (string.IsNullOrWhiteSpace(couponJson)) return;

            try
            {
                var used = System.Text.Json.JsonSerializer.Deserialize<UsedCouponDto>(couponJson);
                if (used?.id > 0)
                {
                    var rec = await _db.CustomerCouponsRecords
                        .FirstOrDefaultAsync(x => x.CustomerID == customerId && x.CouponID == used.id);

                    if (rec != null)
                    {
                        rec.IsUsed = true;            // bool?
                        rec.UsedTime = DateTime.Now;  // 記錄使用時間
                        await _db.SaveChangesAsync();
                    }
                }
            }
            catch { /* ignore bad session */ }
            finally
            {
                // 無論成功與否都清掉，避免重覆使用
                HttpContext.Session.Remove("CART_COUPON");
            }
        }

        private class UsedCouponDto
        {
            public int id { get; set; }
            public string? code { get; set; }
            public string? type { get; set; }   // percent / fixed（可有可無）
            public decimal? value { get; set; } // 可有可無
        }

        private class AppliedCoupon
        {
            public int couponId { get; set; }
            public string? code { get; set; }
            public string? type { get; set; }
            public decimal? value { get; set; }
        }

        public class CardPayDto
		{
			public string Card { get; set; } = "";
			public string Exp { get; set; } = "";
			public string Cvc { get; set; } = "";
			public string Invoice { get; set; } = "";
		}

        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer([FromForm] int customerId)
        {
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();

            var email = await _db.CustomerProfile
                .Where(c => c.CustomerID == cid)
                .Select(c => c.Email)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { ok = false, error = "找不到客戶 Email" });

            var html = $@"
<table width='100%' cellpadding='0' cellspacing='0' style='font-family:Arial,sans-serif;color:#333'>
<tr><td align='center'>
  <table width='600' style='max-width:600px;border:1px solid #eee;border-radius:8px;overflow:hidden'>
    <tr><td style='background:#0d6efd;color:#fff;padding:16px 20px;font-size:18px;font-weight:700'>
      匯款資訊通知
    </td></tr>
    <tr><td style='padding:20px;line-height:1.7'>
      親愛的顧客您好，<br>
      感謝您的訂購，請於 3 日內完成匯款以保留名額：<br><br>
      <b>銀行：</b> 台灣銀行 004<br>
      <b>帳號：</b> 123-456-789-012<br>
      <b>戶名：</b> 貓爪足跡股份有限公司<br><br>
      完成後請回覆此信件或提供匯款後五碼，以便對帳。<br><br>
      祝您旅途愉快！
    </td></tr>
    <tr><td style='background:#f8f9fa;padding:16px 20px;font-size:12px;color:#666'>
      本信由系統發送，請勿直接回覆；若有問題請來信客服：support@example.com
    </td></tr>
  </table>
</td></tr></table>";

            await _sender.SendAsync(email, "【匯款資訊】貓爪足跡", html);
            return Ok(new { ok = true });
        }
        private string CashierUrl => _opt.IsStage
            ? "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5"
            : "https://payment.ecpay.com.tw/Cashier/AioCheckOut/V5";        // 1) 前端按「信用卡付款」→ 從購物車讀資料 → 呼叫 Create (回自動送出的 GoECPay View)

        [HttpPost("CheckoutCredit")]
        public IActionResult CheckoutCredit([FromForm] int[] productIds)
        {
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();

            if (productIds == null || productIds.Length == 0)
                return BadRequest("未收到要付款的商品。請至少勾選一項。");

            // 讀購物車 → 過濾出勾選項目 → 回 MockPay（模擬付款頁）
            var cartJson = HttpContext.Session.GetString(CartKey);
            var items = string.IsNullOrWhiteSpace(cartJson)
                ? new List<CartItem>()
                : System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new();

            var sel = items.Where(x => productIds.Contains(x.ProductId)).ToList();
            if (sel.Count == 0)
                return BadRequest("勾選的商品不在購物車內（可能已移除或不同帳號的購物車）。");

            var vm = new MockPayVm
            {
                Items = sel.Select(s => new MockPayVm.PayItem
                {
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    Qty = s.Qty,
                    Price = s.Price
                }).ToList(),
                Total = sel.Sum(s => s.Price * s.Qty)
            };
            
            HttpContext.Session.SetString(PayItemsKey(cid), string.Join(",", productIds));

            return View("MockPay", vm);


            /* ★ 真金流流程（保留參考，不執行）
            // 讀購物車 Session => 存成快照 snapKey
            //var cartJson = HttpContext.Session.GetString("CART_ITEMS");
            //if (string.IsNullOrWhiteSpace(cartJson)) return BadRequest("購物車是空的");

            //var snapKey = "CART_SNAP_" + Guid.NewGuid().ToString("N");
            //HttpContext.Session.SetString(snapKey, cartJson);
            return RedirectToAction(nameof(Create), new { customerId = cid, snapKey });*/

        }

        // 2) 建立綠界交易，送出自動表單
        [HttpGet("Create")]
        public IActionResult Create([FromQuery] int customerId, [FromQuery] string snapKey)
        {
            if (customerId != CurrentCustomerId) return Forbid();
            var cartJson = HttpContext.Session.GetString(snapKey);
            if (string.IsNullOrWhiteSpace(cartJson)) return BadRequest("購物車快照遺失");

            var items = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new();
            if (items.Count == 0) return BadRequest("購物車為空");

            var total = items.Sum(x => x.Price * x.Qty);
            if (total <= 0) return BadRequest("金額錯誤");

            var itemNames = string.Join("#", items.Select(x => $"{x.ProductName}x{x.Qty}"));

            var dict = new SortedDictionary<string, string>
            {
                ["MerchantID"] = _opt.MerchantID,
                ["MerchantTradeNo"] = $"C{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}",
                ["MerchantTradeDate"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                ["PaymentType"] = "aio",
                ["TotalAmount"] = total.ToString(),
                ["TradeDesc"] = "Cat Paw Footprint 訂單付款",
                ["ItemName"] = itemNames,
                ["ReturnURL"] = _opt.ReturnURL,
                ["OrderResultURL"] = _opt.OrderResultURL,
                ["ClientBackURL"] = _opt.ClientBackURL,
                ["ChoosePayment"] = "Credit",
                ["EncryptType"] = "1"
            };

            // ★ 自訂欄位固定這樣對應：CF1=customerId、CF2=snapKey
            dict["CustomField1"] = customerId.ToString();
            dict["CustomField2"] = snapKey;

            dict["CheckMacValue"] = MakeCheckMac(dict, _opt.HashKey, _opt.HashIV);

            return View("~/Areas/CustomersArea/Views/Payment/GoECPay.cshtml", new GoECPayVm { Action = CashierUrl, Fields = dict });
        }


        // 3) 綠界後端回呼：驗證成功 → 這裡才建立訂單、狀態=已付款
        [AllowAnonymous]
        [HttpPost("Return")]
        public async Task<IActionResult> Return()
        {
            var form = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());
            if (!form.TryGetValue("CheckMacValue", out var macRtn))
                return Content("0|CheckMacValue missing");

            var verify = new SortedDictionary<string, string>(
                form.Where(kv => kv.Key != "CheckMacValue").ToDictionary(k => k.Key, v => v.Value)
            );
            var mac = MakeCheckMac(verify, _opt.HashKey, _opt.HashIV);
            if (!string.Equals(mac, macRtn, StringComparison.OrdinalIgnoreCase))
                return Content("0|CheckMac錯誤");

            var ok = form.TryGetValue("RtnCode", out var code) && code == "1";
            if (!ok) return Content("1|OK"); // 失敗就不建單，但仍回 1|OK

            // ★ 對應 Create 時的設定
            var customerId = form.TryGetValue("CustomField1", out var cf1) ? int.Parse(cf1) : 0;
            var snapKey = form.TryGetValue("CustomField2", out var cf2) ? cf2 : "";

            if (customerId > 0 && !string.IsNullOrWhiteSpace(snapKey))
            {
                var cartJson = HttpContext.Session.GetString(snapKey);
                if (!string.IsNullOrWhiteSpace(cartJson))
                {
                    var items = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new();
                    var now = DateTime.Now;

                    foreach (var it in items)
                    {
                        _db.CustomerOrders.Add(new Cat_Paw_Footprint.Models.CustomerOrders
                        {
                            CustomerID = customerId,
                            ProductID = it.ProductId,
                            OrderStatusID = 1, // 已付款
                            TotalAmount = it.Price * it.Qty,
                            CreateTime = now,
                            UpdateTime = now
                        });
                    }
                    await _db.SaveChangesAsync();

                    // 清掉快照與購物車
                    HttpContext.Session.Remove(snapKey);
                    HttpContext.Session.Remove("CART_ITEMS");

                    // ★ 重算升等
                    await _levelSvc.RecalculateAndUpdateAsync(customerId);
                }
            }

            return Content("1|OK");
        }

        // 4) 前端導回頁：導到「我的訂單」
        [HttpPost("Result")]
        public IActionResult Result() => Redirect("/CustomersArea/Orders");

        // ===== 工具/型別 =====

        private static string MakeCheckMac(SortedDictionary<string, string> fields, string hashKey, string hashIV)
        {
            var sb = new StringBuilder();
            sb.Append($"HashKey={hashKey}");
            foreach (var kv in fields.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)))
                sb.Append($"&{kv.Key}={kv.Value}");
            sb.Append($"&HashIV={hashIV}");

            var encoded = System.Web.HttpUtility.UrlEncode(sb.ToString()).ToLower();
            var fixedStr = encoded.Replace("%2d", "-").Replace("%5f", "_").Replace("%2e", ".")
                                  .Replace("%21", "!").Replace("%2a", "*").Replace("%28", "(").Replace("%29", ")");

            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(fixedStr));
            var outSB = new StringBuilder();
            foreach (var b in bytes) outSB.Append(b.ToString("X2"));
            return outSB.ToString();
        }

        [HttpPost("CheckoutCreditByOrder")]
        public async Task<IActionResult> CheckoutCreditByOrder([FromForm] int orderId)
        {
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();

            var o = await _db.CustomerOrders
                .AsNoTracking()
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.OrderID == orderId && x.CustomerID == cid);

            if (o == null) return NotFound("訂單不存在");
            if (o.OrderStatusID != 2 /*未付款*/) return BadRequest("此訂單不可付款");

            // 做成 MockPayVm（等同於購物車選的那種）
            var vm = new MockPayVm
            {
                Items = new List<MockPayVm.PayItem>{
            new() {
                ProductId = o.ProductID ?? 0,
                ProductName = o.Product?.ProductName ?? $"商品 {o.ProductID}",
                Qty = 1,
                Price = (int)(o.TotalAmount ?? 0)
            }
        },
                Total = (int)(o.TotalAmount ?? 0)
            };

            // 存一下「這次要標記的訂單 id」到 Session，PayMock 成功後設成已付款
            HttpContext.Session.SetString("PAY_ORDER_ID", orderId.ToString());
            HttpContext.Session.Remove("PAY_SELECTED_IDS"); // 確保不跟購物車付款混在一起

            return View("MockPay", vm);
        }
        private class CartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = "";
            public int Price { get; set; }
            public int Qty { get; set; }
        }
        public class MockPayVm
        {
            public List<PayItem> Items { get; set; } = new();
            public int Total { get; set; }
            public class PayItem
            {
                public int ProductId { get; set; }
                public string ProductName { get; set; } = "";
                public int Qty { get; set; }
                public int Price { get; set; }
            }
        }

        public class GoECPayVm
		{
			public string Action { get; set; } = "";
			public SortedDictionary<string, string> Fields { get; set; } = new();
		}
	}
}

