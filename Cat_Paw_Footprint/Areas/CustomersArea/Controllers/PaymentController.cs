using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Areas.Order.Services;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")] // ← 必須和資料夾名一樣
	[Route("CustomersArea/[controller]")] // → URL 會是 /CustomersArea/Payment/...
	public class PaymentController : Controller
	{
		private readonly webtravel2Context _db;
		private readonly ECPayOptions _opt;
        private readonly ICustomerLevelService _levelSvc;
        private readonly IEmailSender _sender;
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

		public IActionResult PayMock([FromBody] CardPayDto dto)
		{
			// TODO：這裡可呼叫金流沙盒 API；示範直接通過
			if (string.IsNullOrWhiteSpace(dto.Card) || string.IsNullOrWhiteSpace(dto.Exp) || string.IsNullOrWhiteSpace(dto.Cvc))
				return BadRequest(new { ok = false, error = "資料不完整" });

			// 這裡你可以：建立付款紀錄、更新訂單狀態=已付款 …
			return Ok(new { ok = true });
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
            var email = await _db.CustomerProfile
                .Where(c => c.CustomerID == customerId)
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
        public IActionResult CheckoutCredit([FromForm] int customerId)
        {
            var cartJson = HttpContext.Session.GetString("CART_ITEMS");
            if (string.IsNullOrWhiteSpace(cartJson))
                return BadRequest("購物車是空的");

            var snapKey = "CART_SNAP_" + Guid.NewGuid().ToString("N");
            HttpContext.Session.SetString(snapKey, cartJson);

            return RedirectToAction(nameof(Create), new { customerId, snapKey });
        }

        // 2) 建立綠界交易，送出自動表單
        [HttpGet("Create")]
        public IActionResult Create([FromQuery] int customerId, [FromQuery] string snapKey)
        {
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

            return View("GoECPay", new GoECPayVm { Action = CashierUrl, Fields = dict });
        }


        // 3) 綠界後端回呼：驗證成功 → 這裡才建立訂單、狀態=已付款
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

        private class CartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = "";
            public int Price { get; set; }
            public int Qty { get; set; }
        }

        public class GoECPayVm
		{
			public string Action { get; set; } = "";
			public SortedDictionary<string, string> Fields { get; set; } = new();
		}
	}
}

