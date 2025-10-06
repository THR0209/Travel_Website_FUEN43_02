using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerArea.Order.Controllers
{
    [Area("CustomerArea")]
    [Route("CustomerArea/Order/[controller]")]
    public class CartController : Controller
    {
        private readonly webtravel2Context _db;
        public CartController(webtravel2Context db) => _db = db;

        const string CART_KEY = "CART_ITEMS";

        private class CartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = "";
            public int Price { get; set; }           // 以 int 當金額
            public int Qty { get; set; }
            public string ImageUrl { get; set; } = "";
        }

        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CART_KEY);
            return string.IsNullOrEmpty(json) ? new List<CartItem>() :
                   JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }
        private void SaveCart(List<CartItem> items)
            => HttpContext.Session.SetString(CART_KEY, JsonSerializer.Serialize(items));

        // 頁面
        [HttpGet("")]
        public IActionResult Index() => View("~/Areas/CustomerArea/Order/Views/Cart/Index.cshtml");
        // API：讀取購物車
        [HttpGet("api")]
        public IActionResult GetCartApi()
        {
            var items = GetCart();
            var total = items.Sum(x => x.Price * x.Qty);
            return Ok(new { items, total });
        }

        // API：加入購物車（body: productId, qty）
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
                    Price = (int)(p.ProductPrice ?? 0), // 請依你的欄位調整
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

        // API：更新數量
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

        // API：移除
        [HttpPost("remove")]
        public IActionResult Remove([FromForm] int productId)
        {
            var items = GetCart();
            items.RemoveAll(x => x.ProductId == productId);
            SaveCart(items);
            return Ok(new { ok = true });
        }

        // API：清空
        [HttpPost("clear")]
        public IActionResult Clear()
        {
            SaveCart(new List<CartItem>());
            return Ok(new { ok = true });
        }

        // API：結帳（建立 N 筆 CustomerOrders），用 customerId 代表登入顧客
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromForm] int customerId)
        {
            var items = GetCart();
            if (items.Count == 0) return BadRequest(new { ok = false, error = "購物車是空的" });

            var now = DateTime.Now;
            foreach (var it in items)
            {
                var order = new CustomerOrders
                {
                    CustomerID = customerId,
                    ProductID = it.ProductId,
                    OrderStatusID = 2, // 預設「未付款」
                    TotalAmount = it.Price * it.Qty,
                    CreateTime = now,
                    UpdateTime = now
                };
                _db.CustomerOrders.Add(order);
            }
            await _db.SaveChangesAsync();

            SaveCart(new List<CartItem>());
            return Ok(new { ok = true });
        }
    }
}
