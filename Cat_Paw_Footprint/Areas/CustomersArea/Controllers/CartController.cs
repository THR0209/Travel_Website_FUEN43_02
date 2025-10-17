using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Route("CustomersArea/[controller]")]
	public class CartController : Controller
	{
		private readonly webtravel2Context _db;
		public CartController(webtravel2Context db) => _db = db;

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
			return string.IsNullOrEmpty(json)
				? new List<CartItem>()
				: (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());
		}
		private void SaveCart(List<CartItem> items)
			=> HttpContext.Session.SetString(CART_KEY, JsonSerializer.Serialize(items));

		[HttpGet("")]
		[HttpGet("Index")]
		public IActionResult Index() => View(); // Views/Cart/Index.cshtml

		// 讀購物車
		[HttpGet("api")]
		public IActionResult GetCartApi()
		{
			var items = GetCart();
			var total = items.Sum(x => x.Price * x.Qty);
			return Ok(new { items, total });
		}

		// 加入購物車（以資料庫 ProductID 為準）
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

		[HttpPost("update")]
		public IActionResult Update([FromForm] int productId, [FromForm] int qty)
		{
			var items = GetCart();
			var it = items.FirstOrDefault(x => x.ProductId == productId);
			if (it == null) return NotFound(new { ok = false, error = "商品不在購物車" });
			it.Qty = Math.Max(1, qty);
			SaveCart(items);
			return Ok(new { ok = true });
		}

		[HttpPost("remove")]
		public IActionResult Remove([FromForm] int productId)
		{
			var items = GetCart();
			items.RemoveAll(x => x.ProductId == productId);
			SaveCart(items);
			return Ok(new { ok = true });
		}

		[HttpPost("clear")]
		public IActionResult Clear()
		{
			SaveCart(new List<CartItem>());
			return Ok(new { ok = true });
		}

		// 結帳 → 建立未付款訂單，轉導到訂單頁
		[HttpPost("checkout")]
		public async Task<IActionResult> Checkout([FromForm] int customerId)
		{
			var items = GetCart();
			if (items.Count == 0) return BadRequest(new { ok = false, error = "購物車是空的" });

			var now = DateTime.Now;
			foreach (var it in items)
			{
				_db.CustomerOrders.Add(new Models.CustomerOrders
				{
					CustomerID = customerId,      // 後續可改用登入 ID
					ProductID = it.ProductId,
					OrderStatusID = 2,            // 未付款
					TotalAmount = it.Price * it.Qty,
					CreateTime = now,
					UpdateTime = now
				});
			}
			await _db.SaveChangesAsync();

			SaveCart(new List<CartItem>());
			return Ok(new { ok = true });
		}
		// 批次刪除（接收 body: [1,2,3]）
		[HttpPost("batch-remove")]
		public IActionResult BatchRemove([FromBody] List<int> ids)
		{
			var items = GetCart();
			items.RemoveAll(x => ids.Contains(x.ProductId));
			SaveCart(items);
			return Ok(new { ok = true });
		}

		// 折價券：套用
		[HttpPost("apply-coupon")]
		public IActionResult ApplyCoupon([FromForm] string code)
		{
			// DEMO 規則：CODE10 = 10% off；MINUS500 = 減 500
			var coupon = new { Code = code, Percent = 0m, Minus = 0m, Hint = "" };

			if (string.Equals(code, "CODE10", StringComparison.OrdinalIgnoreCase))
				coupon = new { Code = code, Percent = 0.10m, Minus = 0m, Hint = "已套用 9 折" };
			else if (string.Equals(code, "MINUS500", StringComparison.OrdinalIgnoreCase))
				coupon = new { Code = code, Percent = 0m, Minus = 500m, Hint = "已折抵 NT$ 500" };
			else
				return BadRequest(new { ok = false, error = "折價碼無效" });

			HttpContext.Session.SetString("CART_COUPON", System.Text.Json.JsonSerializer.Serialize(coupon));
			return Ok(new { ok = true, hint = coupon.Hint });
		}

		// 折價券：清除
		[HttpPost("clear-coupon")]
		public IActionResult ClearCoupon()
		{
			HttpContext.Session.Remove("CART_COUPON");
			return Ok(new { ok = true });
		}
		[HttpGet("first-product-id")]
		public async Task<IActionResult> FirstProductId()
		{
			var id = await _db.Products
				.AsNoTracking()
				.OrderBy(p => p.ProductID)
				.Select(p => p.ProductID)
				.FirstOrDefaultAsync();

			if (id == 0) return NotFound(new { ok = false, error = "沒有商品資料" });
			return Ok(new { productId = id });
		}
	}
}
