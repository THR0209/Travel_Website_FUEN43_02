using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Areas.Order.Services;

namespace Cat_Paw_Footprint.Areas.CustomersArea.CartControllers
{
    [Area("CustomersArea")]
    [Route("CustomersArea/[controller]")]
    public class CartController : Controller
    {
        private readonly webtravel2Context _db;
        private readonly IEmailSender _sender; // ← 寄信
        public CartController(webtravel2Context db, IEmailSender sender)
        {
            _db = db;
            _sender = sender;
        }

        private const string CART_KEY = "CART_ITEMS";

        private class CartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = "";
            public int Price { get; set; }
            public int Qty { get; set; }
            public string ImageUrl { get; set; } = "";
        }

        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CART_KEY);
            return string.IsNullOrEmpty(json)? new List<CartItem>()
                : (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());
        }

        private void SaveCart(List<CartItem> items)
            => HttpContext.Session.SetString(CART_KEY, JsonSerializer.Serialize(items));

        // 頁面
        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index() => View();

        // 讀購物車
        [HttpGet("api")]
        public IActionResult GetCartApi()
        {
            var items = GetCart();
            var total = items.Sum(x => x.Price * x.Qty);
            return Ok(new { items, total });
        }

        // 加入購物車（body: productId, qty）
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromForm] int productId, [FromForm] int qty = 1)
        {
            var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductID == productId);
            if (p == null) return NotFound();

            var items = GetCart();
            var exist = items.FirstOrDefault(x => x.ProductId == productId);
            if (exist == null)
            {
                items.Add(new CartItem
                {
                    ProductId = p.ProductID,
                    ProductName = p.ProductName ?? $"商品 {p.ProductID}",
                    Price = p.ProductPrice ?? 0,
                    Qty = Math.Max(1, qty),
                    ImageUrl = p.ProductImage != null
                        ? "data:image/png;base64," + Convert.ToBase64String(p.ProductImage)
                        : Url.Content("~/images/NoImage.png")
                });
            }
            else
            {
                exist.Qty += Math.Max(1, qty);
            }
            SaveCart(items);
            return Ok(new { ok = true });
        }

        // 更新數量
        [HttpPost("update")]
        public IActionResult Update([FromForm] int productId, [FromForm] int qty)
        {
            var items = GetCart();
            var it = items.FirstOrDefault(x => x.ProductId == productId);
            if (it == null) return NotFound();
            it.Qty = Math.Max(1, qty);
            SaveCart(items);
            return Ok(new { ok = true });
        }

        // 移除
        [HttpPost("remove")]
        public IActionResult Remove([FromForm] int productId)
        {
            var items = GetCart();
            items.RemoveAll(x => x.ProductId == productId);
            SaveCart(items);
            return Ok(new { ok = true });
        }

        // 清空
        [HttpPost("clear")]
        public IActionResult Clear()
        {
            SaveCart(new List<CartItem>());
            return Ok(new { ok = true });
        }

        // 結帳：以 customerId 建立 N 筆 CustomerOrders，狀態先設「未付款」(2)
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromForm] int customerId, [FromForm] string method)
        {
            var items = GetCart();
            if (items.Count == 0) return BadRequest(new { ok = false, error = "購物車是空的" });
            if (customerId <= 0) return BadRequest(new { ok = false, error = "缺少客戶編號" });

            var now = DateTime.Now;
            var createdIds = new List<int>();
            int total = 0;

            foreach (var it in items)
            {
                var order = new CustomerOrders
                {
                    CustomerID = customerId,
                    ProductID = it.ProductId,
                    // 狀態：2=未付款（你自己的對照）
                    OrderStatusID = 2,
                    TotalAmount = it.Price * it.Qty,
                    CreateTime = now,
                    UpdateTime = now
                };
                total += order.TotalAmount ?? 0;
                _db.CustomerOrders.Add(order);
                await _db.SaveChangesAsync();
                createdIds.Add(order.OrderID);
            }

            // 清空購物車
            SaveCart(new List<CartItem>());

            if (string.Equals(method, "transfer", StringComparison.OrdinalIgnoreCase))
            {
                // 轉帳：寄信給客戶（取客戶 Email）
                var cp = await _db.CustomerProfile.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CustomerID == customerId);

                var to = cp?.Email ?? "";
                var name = cp?.CustomerName ?? $"客戶 {customerId}";
                var bankInfoHtml = $@"
<h3>匯款資訊</h3>
<ul>
  <li>銀行：台灣銀行 (004)</li>
  <li>帳號：12345678901234</li>
  <li>戶名：貓爪足跡股份有限公司</li>
  <li>總金額：NT$ {total:N0}</li>
</ul>
<p>請於 3 日內完成匯款並回覆此信，我們將為您加速出貨（或安排行程）。謝謝！</p>";

                var codeList = string.Join(", ", createdIds.Select(id => $"ORD-{now:yyyyMMdd}-{id}"));
                var subject = $"[匯款通知] 您的訂單：{codeList}";
                var body = $@"
<p>親愛的 {name} 您好：</p>
<p>您已建立以下訂單（尚未付款）：{codeList}</p>
{bankInfoHtml}
<p>貓爪足跡 敬上</p>";

                if (!string.IsNullOrWhiteSpace(to))
                {
                    await _sender.SendAsync(to, subject, body);
                }

                // 回前端：導去「我的訂單」頁
                var redirect = Url.Action("Index", "Orders", new { area = "CustomersArea" }) ?? "/CustomersArea/Orders";
                return Ok(new { ok = true, redirect });
            }
            else
            {
                // 信用卡：導去模擬刷卡頁（把總金額與訂單 ids 帶過去）
                var redirect = Url.Action("MockPay", "Payment", new
                {
                    area = "CustomersArea",
                    total,
                    ids = string.Join(",", createdIds)
                }) ?? "/CustomersArea/Payment/MockPay";
                return Ok(new { ok = true, redirect });
            }
        }
        // （選配）為了測試方便，快速加入一筆 demo 商品
        [HttpPost("add-demo")]
        public async Task<IActionResult> AddDemo()
        {
            var p = await _db.Products.AsNoTracking().OrderBy(x => x.ProductID).FirstOrDefaultAsync();
            if (p == null) return NotFound("資料庫沒有商品，可先建立 Products 資料");
            return await Add(p.ProductID, 1);
        }
    }
}