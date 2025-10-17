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
		public PaymentController(webtravel2Context db, IEmailSender sender, IOptions<ECPayOptions> opt)
		{
			_db = db;
			_sender = sender;
			_opt = opt.Value;
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
			// 1) 建立訂單（或僅標記付款方式=匯款）
			// 2) 取客戶 Email
			// 3) 寄信
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
			: "https://payment.ecpay.com.tw/Cashier/AioCheckOut/V5";

		// Step 1: 建立交易，送到綠界
		// 呼叫方式：POST /CustomersArea/Payment/Create  (body: orderId)
		[HttpPost("Create")]
		public async Task<IActionResult> Create([FromForm] int orderId)
		{
			var order = await _db.CustomerOrders
				.Include(x => x.Product)
				.FirstOrDefaultAsync(x => x.OrderID == orderId);
			if (order == null) return NotFound("訂單不存在");

			// 依你實際欄位計算金額
			var total = (int)(order.TotalAmount ?? 0);
			if (total <= 0) return BadRequest("金額不正確");

			// 綠界必填欄位
			var dict = new SortedDictionary<string, string>
			{
				["MerchantID"] = _opt.MerchantID,
				["MerchantTradeNo"] = $"C{order.OrderID}{DateTime.Now:yyyyMMddHHmmss}", // 商店訂單編號：要唯一
				["MerchantTradeDate"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
				["PaymentType"] = "aio",
				["TotalAmount"] = total.ToString(),
				["TradeDesc"] = "Cat Paw Footprint 訂單付款",
				["ItemName"] = order.Product?.ProductName ?? $"商品{order.ProductID}",
				["ReturnURL"] = _opt.ReturnURL,           // 後端
				["OrderResultURL"] = _opt.OrderResultURL, // 前端
				["ClientBackURL"] = _opt.ClientBackURL,
				["ChoosePayment"] = "Credit",
				["EncryptType"] = "1"  // SHA256
			};

			// 自訂欄位（會原封不動回傳）
			dict["CustomField1"] = order.OrderID.ToString();

			// 算 CheckMacValue
			var mac = MakeCheckMac(dict, _opt.HashKey, _opt.HashIV);
			dict["CheckMacValue"] = mac;

			// 回傳一個自動送出到綠界的表單頁
			return View("GoECPay", new GoECPayVm { Action = CashierUrl, Fields = dict });
		}

		// Step 2: 後端回呼（Server to Server）
		[HttpPost("Return")]
		public async Task<IActionResult> Return()
		{
			var form = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());

			// 取出回傳的 CheckMacValue
			if (!form.TryGetValue("CheckMacValue", out var returnedMac))
				return Content("0|CheckMacValue missing");

			// 驗證 MAC
			var verifyFields = new SortedDictionary<string, string>(
				form.Where(kv => kv.Key != "CheckMacValue").ToDictionary(k => k.Key, v => v.Value)
			);
			var mac = MakeCheckMac(verifyFields, _opt.HashKey, _opt.HashIV);
			if (!string.Equals(mac, returnedMac, StringComparison.OrdinalIgnoreCase))
				return Content("0|CheckMacValue驗證失敗");

			// 交易結果
			var status = form.TryGetValue("RtnCode", out var rtn) ? rtn : "";
			var ok = status == "1"; // 1=付款成功（信用卡）

			// 取你帶的訂單 id
			var orderId = form.TryGetValue("CustomField1", out var cf1) ? int.Parse(cf1) : 0;
			var order = await _db.CustomerOrders.FirstOrDefaultAsync(x => x.OrderID == orderId);
			if (order != null)
			{
				order.OrderStatusID = ok ? 1 : 4; // 1=已付款、4=錯誤（依你的表設計）
				order.UpdateTime = DateTime.Now;
				await _db.SaveChangesAsync();
				if (ok)
				{
					var customerId = order.CustomerID;

					// 歷史已付款總額（依你「已付款」狀態 ID 調整這裡，我用 1）
					var totalPaid = await _db.CustomerOrders
						.Where(x => x.CustomerID == customerId && x.OrderStatusID == 1)
						.SumAsync(x => (decimal?)x.TotalAmount ?? 0m);

					int newLevel =
						totalPaid >= 30000m ? 3 :
						totalPaid >= 15000m ? 2 :
						totalPaid > 0m ? 1 : 0;

					// 更新 Customers.Level（你的實體名稱若是 Customer，就改成 _db.Customer）
					var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerID == customerId);
					if (customer != null && customer.Level != newLevel)
					{
						customer.Level = newLevel;
						await _db.SaveChangesAsync();
					}
				}
			}
			// 一定要回 "1|OK"，綠界才會認為你有收到
			return Content("1|OK");
		}

		// Step 3: 前端導回頁（付款完成）
		[HttpPost("Result")]
		public IActionResult Result()
		{
			// 這裡可以讀 Request.Form 顯示成功頁（前端）
			// 也可以直接導去「我的訂單」
			return Redirect("/CustomersArea/Orders");
		}

		// 工具：計算 CheckMacValue（SHA256）
		private static string MakeCheckMac(SortedDictionary<string, string> fields, string hashKey, string hashIV)
		{
			// 1) 只取有值的欄位，依 Key 排序，組字串
			var raw = new StringBuilder();
			raw.Append($"HashKey={hashKey}");
			foreach (var kv in fields.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)))
				raw.Append($"&{kv.Key}={kv.Value}");
			raw.Append($"&HashIV={hashIV}");

			// 2) URL encode（小寫），再轉小寫
			var urlEncoded = System.Web.HttpUtility.UrlEncode(raw.ToString()).ToLower();

			// 3) 取代特殊字元（符合綠界規則）
			string toMac = urlEncoded
				.Replace("%2d", "-").Replace("%5f", "_").Replace("%2e", ".")
				.Replace("%21", "!").Replace("%2a", "*").Replace("%28", "(")
				.Replace("%29", ")");

			// 4) SHA256，轉大寫 16 進制
			using var sha = SHA256.Create();
			var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(toMac));
			var sb = new StringBuilder(bytes.Length * 2);
			foreach (var b in bytes) sb.Append(b.ToString("X2"));
			return sb.ToString();
		}
	}

	public class GoECPayVm
	{
		public string Action { get; set; } = "";
		public SortedDictionary<string, string> Fields { get; set; } = new();
	}
}

