using Cat_Paw_Footprint.Areas.Order.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Services;
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
		private readonly IWebHostEnvironment _env;
		private readonly INotificationTriggerService _notifTrigger;
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
		public PaymentController(webtravel2Context db, IEmailSender sender, IOptions<ECPayOptions> opt,
			ICustomerLevelService levelSvc, IConfiguration cfg, IWebHostEnvironment env,INotificationTriggerService notifTrigger)
		{
			_db = db;
			_sender = sender;
			_opt = opt.Value;
			_levelSvc = levelSvc;
			_env = env;
			_notifTrigger = notifTrigger;
		}
		// /CustomersArea/Payment
		[HttpGet("")]              // GET /CustomersArea/Payment
		public IActionResult Index()
				=> View("MockPay");    // 明確指向 Views/Payment/MockPay.cshtml

		[HttpGet("MockPay")]       // GET /CustomersArea/Payment/MockPay
		public IActionResult MockPay()
			=> View();             //        [HttpPost("PayMock")]

		[HttpPost("MockPay")]
		public async Task<IActionResult> MockPay(
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
				return RedirectToAction(nameof(MockPay));
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

				// ✅ 單筆訂單：付款成功通知
				await _notifTrigger.NotifyPaymentSuccessAsync(cid, o.OrderID);


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

			// ✅ 通知訂單成立 + 付款成功
			foreach (var it in _db.CustomerOrders
				.Where(o => o.CustomerID == cid && o.CreateTime == now))
			{
				await _notifTrigger.NotifyOrderCreatedAsync(cid, it.OrderID);
				await _notifTrigger.NotifyPaymentSuccessAsync(cid, it.OrderID);
			}


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
		public async Task<IActionResult> CheckoutCredit([FromForm] int[] productIds)
		{
			var cid = CurrentCustomerId;
			if (cid <= 0) return Unauthorized();

			if (productIds == null || productIds.Length == 0)
				return BadRequest("未收到要付款的商品。請至少勾選一項。");

			var cartJson = HttpContext.Session.GetString($"CART_ITEMS_{cid}");
			var items = string.IsNullOrWhiteSpace(cartJson)
				? new List<CartItem>()
				: System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new();

			var sel = items.Where(x => productIds.Contains(x.ProductId)).ToList();
			if (sel.Count == 0)
				return BadRequest("勾選的商品不在購物車內。");

			var originalTotal = sel.Sum(s => s.Price * s.Qty);

			// 只要 Session 還有 CART_COUPON 才視為要套用
			var couponJson = HttpContext.Session.GetString("CART_COUPON");
			var finalTotal = originalTotal;

			if (!string.IsNullOrWhiteSpace(couponJson))
			{
				try
				{
					var used = System.Text.Json.JsonSerializer.Deserialize<UsedCouponDto>(couponJson);
					if (used != null)
					{
						var v = (decimal)(used.value ?? 0);
						if (used.type == "1" || string.Equals(used.type, "percent", StringComparison.OrdinalIgnoreCase))
						{
							var discountAmt = Math.Floor(originalTotal * (v / 100m));
							finalTotal = (int)Math.Max(0, originalTotal - discountAmt);
						}
						else if (used.type == "2" || string.Equals(used.type, "fixed", StringComparison.OrdinalIgnoreCase))
						{
							finalTotal = (int)Math.Max(0, originalTotal - v);
						}
					}
				}
				catch { /* ignore */ }
			}

			var now = DateTime.Now;
			var newOrders = new List<CustomerOrders>();
			foreach (var it in sel)
			{
				var order = new CustomerOrders
				{
					CustomerID = cid,
					ProductID = it.ProductId,
					OrderStatusID = 2, // 未付款
					TotalAmount = it.Price * it.Qty,
					CreateTime = now,
					UpdateTime = now
				};
				_db.CustomerOrders.Add(order);
				newOrders.Add(order);
			}

			await _db.SaveChangesAsync();

			// 從購物車移除已建立訂單的項目
			items.RemoveAll(x => productIds.Contains(x.ProductId));
			HttpContext.Session.SetString(CartKey, System.Text.Json.JsonSerializer.Serialize(items));
			HttpContext.Session.Remove(PayItemsKey(cid));

			// 若有多筆訂單，這裡我們只處理第一筆的付款。ECPay 一次只能付一筆。
			// 實務上，您可能需要一個機制來引導使用者分別支付或合併訂單。
			// 為求簡化，我們這裡只取第一筆。
			var firstOrder = newOrders.FirstOrDefault();
			if (firstOrder == null)
			{
				// 這理論上不應該發生
				return RedirectToAction("Index", "Cart");
			}

			var snapKey = "CART_SNAP_" + Guid.NewGuid().ToString("N");
			return RedirectToAction(nameof(Create), new { customerId = cid, snapKey = snapKey, orderId = firstOrder.OrderID });
		}

		// 2) 建立綠界交易，送出自動表單
		[HttpGet("Create")]

		public IActionResult Create([FromQuery] int customerId, [FromQuery] string snapKey, [FromQuery] int? orderId)
		{
			if (customerId != CurrentCustomerId) return Forbid();

			List<CartItem> items;
			int total;

			if (orderId.HasValue && orderId.Value > 0)
			{
				// 訂單明細付款：維持你原本的方式，不走折扣
				var o = _db.CustomerOrders.Include(x => x.Product)
						 .FirstOrDefault(x => x.OrderID == orderId.Value && x.CustomerID == customerId);
				if (o == null || o.OrderStatusID != 2) return BadRequest("訂單不存在或不可付款");

				items = new List<CartItem>{
			new(){ ProductId = o.ProductID??0, ProductName = o.Product?.ProductName ?? $"商品 {o.ProductID}", Qty = 1, Price = (int)(o.TotalAmount??0) }
		};
				total = items.Sum(x => x.Price * x.Qty);
			}
			else
			{
				// 購物車付款：用 Pending（折扣後金額）
				var pending = _db.PendingPayments.AsNoTracking()
					.FirstOrDefault(p => p.SnapKey == snapKey && p.CustomerId == customerId && p.Status == 0);
				if (pending == null) return BadRequest("找不到付款快照");

				items = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(pending.ItemsJson) ?? new();
				total = pending.TotalAmount; // ★ 折扣後金額
			}

			if (total <= 0) return BadRequest("金額需 ≥ 1"); // 多一層保險

			var itemNames = string.Join("#", items.Select(x => $"{x.ProductName}x{x.Qty}"));
			if (itemNames.Length > 200) itemNames = itemNames.Substring(0, 200);
			// 把識別資訊帶去 Result（前端導回）
			var ub = new UriBuilder(_opt.OrderResultURL);
			var qs = System.Web.HttpUtility.ParseQueryString(ub.Query);
			qs["customerId"] = customerId.ToString();
			if (!string.IsNullOrWhiteSpace(snapKey)) qs["snapKey"] = snapKey;
			if (orderId.HasValue) qs["orderId"] = orderId.Value.ToString();
			ub.Query = qs.ToString() ?? "";
			var finalOrderResultUrl = ub.ToString();

			var dict = new SortedDictionary<string, string>
			{
				["MerchantID"] = _opt.MerchantID,
				["MerchantTradeNo"] = $"C{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}",
				["MerchantTradeDate"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
				["PaymentType"] = "aio",
				["TotalAmount"] = total.ToString(), // ★ 一定是整數字串
				["TradeDesc"] = "Cat Paw Footprint 訂單付款",
				["ItemName"] = itemNames,
				["ReturnURL"] = _opt.ReturnURL,
				["OrderResultURL"] = finalOrderResultUrl,
				["ClientBackURL"] = _opt.ClientBackURL,
				["ChoosePayment"] = "Credit",
				["EncryptType"] = "1",
				["CustomField1"] = customerId.ToString(),
				["CustomField2"] = snapKey ?? "",
				["CustomField3"] = orderId?.ToString() ?? ""
			};
			if (total <= 0) return BadRequest("金額需 ≥ 1");
			dict["CheckMacValue"] = MakeCheckMac(dict, _opt.HashKey, _opt.HashIV);
			if (_env.IsDevelopment())
			{
				Console.WriteLine($"[ECPay/Create] total={total}, " +
								  $"customerId={customerId}, snapKey={snapKey}, orderId={orderId}");
				// 可選：再印一次你送給綠界的 TotalAmount 與 ItemName（確保是整數字串與正確品名）
				Console.WriteLine($"[ECPay/Create] TotalAmount(dict)={dict["TotalAmount"]}, ItemName={dict["ItemName"]}");
			}
			Console.WriteLine("[Create] Sending fields to ECPay:");
			foreach (var kv in dict)
				Console.WriteLine($"  {kv.Key} = {kv.Value}");
			return View("~/Areas/CustomersArea/Views/Payment/GoECPay.cshtml",
				new GoECPayVm { Action = CashierUrl, Fields = dict });
		}

		// 3) 綠界後端回呼：驗證成功 → 這裡才建立訂單、狀態=已付款
		[AllowAnonymous]
		[HttpPost("Return")]
		public async Task<IActionResult> Return()
		{
			var form = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());

			// CheckMac 驗證（略同你現有）
			if (!form.TryGetValue("CheckMacValue", out var macRtn))
				return Content("0|CheckMacValue missing");
			var verify = new SortedDictionary<string, string>(
				form.Where(kv => !kv.Key.Equals("CheckMacValue", StringComparison.OrdinalIgnoreCase))
					.ToDictionary(k => k.Key, v => v.Value)
			);
			var mac = MakeCheckMac(verify, _opt.HashKey, _opt.HashIV);
			if (!string.Equals(mac, macRtn, StringComparison.OrdinalIgnoreCase))
				return Content("0|CheckMac錯誤");

			var ok = form.TryGetValue("RtnCode", out var code) && code == "1";
			if (!ok) return Content("1|OK");

			int customerId = form.TryGetValue("CustomField1", out var cf1) && int.TryParse(cf1, out var tmpCid) ? tmpCid : 0;
			string snapKey = form.TryGetValue("CustomField2", out var cf2) ? cf2 : "";
			int orderId = form.TryGetValue("CustomField3", out var cf3) && int.TryParse(cf3, out var tmpOid) ? tmpOid : 0;

			var rtnMsg = form.TryGetValue("RtnMsg", out var msg) ? msg : "";
			// ✅ 只在開發模式印 Log
			if (_env.IsDevelopment())
			{
				Console.WriteLine($"[ECPay/Return] RtnCode={code}, RtnMsg={rtnMsg}, " +
								  $"CF1(customerId)={customerId}, CF2(snapKey)={snapKey}, CF3(orderId)={orderId}");
			}
			if (customerId <= 0) return Content("1|OK");

			var now = DateTime.Now;

			if (orderId > 0)
			{
				// ★★★ 從「訂單頁 → 信用卡」：更新既有訂單
				var o = await _db.CustomerOrders.FirstOrDefaultAsync(x => x.OrderID == orderId && x.CustomerID == customerId);
				if (o != null && o.OrderStatusID == 2) // 2=未付款
				{
					o.OrderStatusID = 1;  // 已付款
					o.UpdateTime = now;
					await _db.SaveChangesAsync();

					// 這次不是購物車結帳，通常不動優惠券與購物車
					await _levelSvc.RecalculateAndUpdateAsync(customerId);
				}
				return Content("1|OK");
			}

			// === 沒有 orderId → 視為「購物車結帳」路徑 ===
			if (string.IsNullOrWhiteSpace(snapKey)) return Content("1|OK");

			var pending = await _db.PendingPayments
				.FirstOrDefaultAsync(p => p.SnapKey == snapKey && p.CustomerId == customerId && p.Status == 0);
			if (pending == null) return Content("1|OK");

			var items = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(pending.ItemsJson) ?? new();
			foreach (var it in items)
			{
				_db.CustomerOrders.Add(new CustomerOrders
				{
					CustomerID = customerId,
					ProductID = it.ProductId,
					OrderStatusID = 1,
					TotalAmount = it.Price * it.Qty,
					CreateTime = now,
					UpdateTime = now
				});
			}

			if (!string.IsNullOrWhiteSpace(pending.CouponJson))
			{
				try
				{
					var used = System.Text.Json.JsonSerializer.Deserialize<UsedCouponDto>(pending.CouponJson);
					if (used?.id > 0)
					{
						var rec = await _db.CustomerCouponsRecords
							.FirstOrDefaultAsync(x => x.CustomerID == customerId && x.CouponID == used.id);
						if (rec != null) { rec.IsUsed = true; rec.UsedTime = now; }
					}
				}
				catch { }
			}

			pending.Status = 1;
			_db.PendingPayments.Remove(pending);
			await _db.SaveChangesAsync();

			HttpContext.Session.Remove($"CART_ITEMS_{customerId}");
			await _levelSvc.RecalculateAndUpdateAsync(customerId);

			return Content("1|OK");
		}

		// 4) 前端導回頁：導到「我的訂單」
		[HttpGet("Result")]
		[HttpPost("Result")]
		[AllowAnonymous]
		[IgnoreAntiforgeryToken] // 用綠界 POST 回來不會帶你站的防偽
		public async Task<IActionResult> Result()
		{
			// 1) 收集所有回傳欄位（Query + Form）
			var bag = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (var kv in Request.Query) bag[kv.Key] = kv.Value.ToString();
			if (Request.HasFormContentType)
				foreach (var kv in Request.Form) bag[kv.Key] = kv.Value.ToString();

			// 2) 把值解析出來（兩條路：orderId or snapKey）
			int customerId = (bag.TryGetValue("customerId", out var qCid) && int.TryParse(qCid, out var tmpCid))
							 ? tmpCid : 0;
			string snapKey = bag.TryGetValue("snapKey", out var qSnap) ? qSnap : "";

			if (customerId <= 0 && bag.TryGetValue("CustomField1", out var cf1) && int.TryParse(cf1, out var cfCid))
				customerId = cfCid;
			if (string.IsNullOrWhiteSpace(snapKey) && bag.TryGetValue("CustomField2", out var cf2))
				snapKey = cf2;

			int orderId =
				(bag.TryGetValue("orderId", out var qOid) && int.TryParse(qOid, out var tmpOid)) ? tmpOid :
				(bag.TryGetValue("CustomField3", out var cf3) && int.TryParse(cf3, out var cfOid)) ? cfOid : 0;

			// 3) MAC 驗證（開發期放寬，正式環境嚴格）
			bool macOk = false;
			if (bag.TryGetValue("CheckMacValue", out var macRtn))
			{
				var verify = new SortedDictionary<string, string>(
					bag.Where(kv => !kv.Key.Equals("CheckMacValue", StringComparison.OrdinalIgnoreCase))
					   .ToDictionary(k => k.Key, v => v.Value)
				);
				var mac = MakeCheckMac(verify, _opt.HashKey, _opt.HashIV);
				macOk = string.Equals(mac, macRtn, StringComparison.OrdinalIgnoreCase);
			}
			bool rtnOk = bag.TryGetValue("RtnCode", out var code) ? (code == "1") : true;

			// ★ 在開發環境放寬（避免你測試時什麼都沒發生）
			if (!_env.IsDevelopment())
			{
				if (!macOk || !rtnOk || customerId <= 0)
					return Redirect("/CustomersArea/Orders");
			}
			else
			{
				if (customerId <= 0) // 開發也至少要有 customerId
					return Redirect("/CustomersArea/Orders");
			}

			var now = DateTime.Now;

			// 4) A 路：訂單頁付款（有 orderId）→ 更新「未付款」→「已付款」
			if (orderId > 0)
			{
				var o = await _db.CustomerOrders.FirstOrDefaultAsync(x => x.OrderID == orderId && x.CustomerID == customerId);
				if (o != null && o.OrderStatusID == 2 /*未付款*/)
				{
					o.OrderStatusID = 1;
					o.UpdateTime = now;
					await _db.SaveChangesAsync();
					await _levelSvc.RecalculateAndUpdateAsync(customerId);
				}
				return Redirect("/CustomersArea/Orders");
			}

			// 5) B 路：購物車付款（有 snapKey）→ 用 Pending 建單
			if (!string.IsNullOrWhiteSpace(snapKey))
			{
				var pending = await _db.PendingPayments
					.FirstOrDefaultAsync(p => p.SnapKey == snapKey && p.CustomerId == customerId && p.Status == 0);

				if (pending != null)
				{
					var items = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(pending.ItemsJson) ?? new();
					foreach (var it in items)
					{
						_db.CustomerOrders.Add(new CustomerOrders
						{
							CustomerID = customerId,
							ProductID = it.ProductId,
							OrderStatusID = 1,
							TotalAmount = it.Price * it.Qty,
							CreateTime = now,
							UpdateTime = now
						});
					}

					if (!string.IsNullOrWhiteSpace(pending.CouponJson))
					{
						try
						{
							var used = System.Text.Json.JsonSerializer.Deserialize<UsedCouponDto>(pending.CouponJson);
							if (used?.id > 0)
							{
								var rec = await _db.CustomerCouponsRecords
									.FirstOrDefaultAsync(x => x.CustomerID == customerId && x.CouponID == used.id);
								if (rec != null) { rec.IsUsed = true; rec.UsedTime = now; }
							}
						}
						catch { }
					}
					if (_env.IsDevelopment())
					{
						var rtnMsg = bag.TryGetValue("RtnMsg", out var msg) ? msg : "";
						Console.WriteLine($"[ECPay/Result] macOk={macOk}, rtnOk={rtnOk}, RtnMsg={rtnMsg}, " +
										  $"customerId={customerId}, snapKey={snapKey}, orderId={orderId}");
					}

					pending.Status = 1;
					_db.PendingPayments.Remove(pending);
					await _db.SaveChangesAsync();

					await _levelSvc.RecalculateAndUpdateAsync(customerId);
				}
			}

			return Redirect("/CustomersArea/Orders");
		}

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
			if (o.OrderStatusID != 2) return BadRequest("此訂單不可付款"); // 2=未付款

			// 把「這張訂單」包成 Pending（就像購物車勾選的一樣）
			var sel = new List<CartItem> {
		new CartItem {
			ProductId = o.ProductID ?? 0,
			ProductName = o.Product?.ProductName ?? $"商品 {o.ProductID}",
			Qty = 1,
			Price = (int)(o.TotalAmount ?? 0)
		}
	};

			var snapKey = "CART_SNAP_" + Guid.NewGuid().ToString("N");
			var itemsJson = System.Text.Json.JsonSerializer.Serialize(sel);

			_db.PendingPayments.Add(new PendingPayment
			{
				SnapKey = snapKey,
				CustomerId = cid,
				ItemsJson = itemsJson,
				TotalAmount = sel.Sum(x => x.Price * x.Qty),
				CouponJson = null, // 針對「單筆舊單再付款」通常不再套券
				Status = 0
			});
			await _db.SaveChangesAsync();

			// 走原本 Create（會帶 customerId, snapKey, orderId）
			return RedirectToAction(nameof(Create), new { customerId = cid, snapKey, orderId });
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

