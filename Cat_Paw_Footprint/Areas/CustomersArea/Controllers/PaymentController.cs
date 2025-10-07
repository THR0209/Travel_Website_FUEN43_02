using Cat_Paw_Footprint.Areas.Order.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomersArea.PaymentControllers
{
    [Area("CustomersArea")]
    [Route("CustomersArea/[controller]")]
    public class PaymentController : Controller
    {
        private readonly webtravel2Context _db;
        private readonly IEmailSender _sender;
        public PaymentController(webtravel2Context db, IEmailSender sender)
        {
            _db = db; _sender = sender;
        }
        public class BankTransferDto
        {
            public int CustomerId { get; set; }       // 之後可改成從 User 取
            public string Email { get; set; } = "";
            public string? InvoiceType { get; set; }  // 發票資訊（後面信用卡也沿用）
            public string? TaxId { get; set; }
        }

        [HttpPost("BankTransfer")]
        public async Task<IActionResult> BankTransfer([FromBody] BankTransferDto dto)
        {
            // 1) 基本驗證
            if (dto.CustomerId <= 0 || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { ok = false, error = "參數不完整" });

            // 2) 取購物車（沿用 Cart 的 session 或者另外傳 items，這裡示例直接從 DB 建立 1 筆測試）
            var now = DateTime.Now;
            var cartItems = HttpContext.Session.GetString("CART_ITEMS");
            if (string.IsNullOrEmpty(cartItems))
                return BadRequest(new { ok = false, error = "購物車是空的" });

            var items = System.Text.Json.JsonSerializer.Deserialize<List<dynamic>>(cartItems) ?? new();

            // 3) 建立訂單（每個商品一筆或合成一筆，看你規格。這裡用「每個商品一筆」）
            var createdOrders = new List<CustomerOrders>();
            foreach (var it in items)
            {
                var order = new CustomerOrders
                {
                    CustomerID = dto.CustomerId,
                    ProductID = (int)it.GetProperty("ProductId").GetInt32(),
                    OrderStatusID = 2,                   // 2=未付款
                    TotalAmount = (int)it.GetProperty("Price").GetInt32() * (int)it.GetProperty("Qty").GetInt32(),
                    CreateTime = now,
                    UpdateTime = now
                };
                _db.CustomerOrders.Add(order);
                createdOrders.Add(order);
            }
            await _db.SaveChangesAsync();

            // 清空購物車
            HttpContext.Session.Remove("CART_ITEMS");

            // 4) 寄信（銀行帳號可放 appsettings 或 DB）
            var bankInfo = @"銀行：XXX 商業銀行 代碼 999
分行：台北分行
戶名：貓爪足跡股份有限公司
帳號：999-999-999999";
            var lines = new List<string> {
            "親愛的顧客您好：",
            "",
            "您選擇「轉帳付款」，以下是匯款資訊：",
            bankInfo,
            "",
            "您的訂單："
        };
            foreach (var o in createdOrders)
            {
                var code = $"ORD-{o.CreateTime:yyyyMMdd-HHmmss}-{o.OrderID}";
                lines.Add($"• {code} 金額 NT$ {(o.TotalAmount ?? 0):N0}");
            }
            lines.Add("");
            lines.Add("請於 3 天內完成付款並回覆帳號後五碼，我們將盡速為您出貨。");
            lines.Add("貓爪足跡 敬上");

            await _sender.SendAsync(dto.Email, "轉帳付款資訊", string.Join("<br/>", lines)); // 用 HTML 換行

            return Ok(new { ok = true, count = createdOrders.Count });
        }
        // GET: /CustomerArea/Order/Payment/MockPay?total=12345&ids=1,2,3
        [HttpGet("MockPay")]
        public IActionResult MockPay([FromQuery] int total, [FromQuery] string ids)
        {
            ViewData["Total"] = total;
            ViewData["Ids"] = ids ?? "";
            return View();
        }

        // POST: /CustomerArea/Order/Payment/Confirm
        [HttpPost("Confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm([FromForm] string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return RedirectToAction("Index", "Orders", new { area = "CustomersArea" });

            var idArr = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => int.TryParse(s, out var x) ? x : 0)
                           .Where(x => x > 0)
                           .ToArray();

            if (idArr.Length == 0) return RedirectToAction("Index", "Orders", new { area = "CustomersArea" });

            var orders = await _db.CustomerOrders
                .Where(o => idArr.Contains(o.OrderID))
                .ToListAsync();

            var now = DateTime.Now;
            foreach (var o in orders)
            {
                o.OrderStatusID = 1; // 1 = 已付款（依你的資料表）
                o.UpdateTime = now;
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Orders", new { area = "CustomersArea" });
        }
    }
}
