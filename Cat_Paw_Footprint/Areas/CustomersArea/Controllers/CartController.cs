using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Route("CustomersArea/[controller]")]
    [Authorize(AuthenticationSchemes = "CustomerAuth")]
    public class CartController : Controller
	{
		private readonly webtravel2Context _db;
		public CartController(webtravel2Context db) => _db = db;
        private int CurrentCustomerId =>
       int.TryParse(User.FindFirst("CustomerId")?.Value, out var id) ? id : 0;
        private string CartKey => $"CART_ITEMS_{CurrentCustomerId}";

        private class CartItem
		{
			public int ProductId { get; set; }
			public string ProductName { get; set; } = "";
			public int Price { get; set; }
			public int Qty { get; set; }
			public string ImageUrl { get; set; } = "";
		}

        private class CouponSession
        {
            public int id { get; set; }
            public string? code { get; set; }
            public int type { get; set; }
            public decimal value { get; set; }
            public string? hint { get; set; }
        }

        private List<CartItem> GetCart()
        {
            if (CurrentCustomerId <= 0) return new List<CartItem>(); // 或 throw Unauthorized
            var json = HttpContext.Session.GetString(CartKey);
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());
        }
        private void SaveCart(List<CartItem> items)
        {
            if (CurrentCustomerId <= 0) return;
            HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(items));
        }

        [HttpGet("")]
		[HttpGet("Index")]
		public IActionResult Index() => View(); // Views/Cart/Index.cshtml

		// 讀購物車
		[HttpGet("api")]
		public IActionResult GetCartApi()
		{
			var items = GetCart();

            CouponSession? coupon = null;
            var cj = HttpContext.Session.GetString("CART_COUPON");
            if (!string.IsNullOrWhiteSpace(cj))
            {
                coupon = JsonSerializer.Deserialize<CouponSession>(cj);
            }

            decimal total = 0;
            bool isFirstItem = true;

            // This logic now exactly mirrors the Checkout method to ensure consistency
            foreach (var it in items)
            {
                decimal finalAmount = it.Price * it.Qty;
                if (coupon != null)
                {
                    if (coupon.type == 1) // Percentage
                    {
                        finalAmount *= coupon.value;
                    }
                    else // Fixed amount
                    {
                        if (isFirstItem)
                        {
                            finalAmount -= coupon.value;
                            isFirstItem = false;
                        }
                    }
                    finalAmount = Math.Max(0, finalAmount); // Match Checkout logic
                }
                total += (int)finalAmount; // Match Checkout logic (per-item rounding)
            }
            
            total = Math.Max(0, total);

            // For display, we still need the original coupon object format
            object? couponForDisplay = null;
            if (!string.IsNullOrWhiteSpace(cj))
                couponForDisplay = JsonSerializer.Deserialize<object>(cj);

            return Ok(new { items, total = (int)total, coupon = couponForDisplay });
		}


		// 加入購物車（以資料庫 ProductID 為準）
		[HttpPost("add")]
		public async Task<IActionResult> Add([FromForm] int productId, [FromForm] int people = 1)
		{
			// 後端防呆
			var qty = Math.Max(1, people);

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
					Qty = qty,
					ImageUrl = p.ProductImage != null
						? "data:image/png;base64," + Convert.ToBase64String(p.ProductImage)
						: Url.Content("~/images/NoImage.png")
				});
			}
			else
			{
				// 若重複加入相同商品，直接加總數量
				exist.Qty += qty;
			}

			SaveCart(items);
			return Ok(new { ok = true, count = items.Count });
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
        public async Task<IActionResult> Checkout()
        {
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();

            var items = GetCart();
            if (items.Count == 0) return BadRequest(new { ok = false, error = "購物車是空的" });

            // Handle coupon logic
            CouponSession? coupon = null;
            var cj = HttpContext.Session.GetString("CART_COUPON");
            if (!string.IsNullOrWhiteSpace(cj))
            {
                coupon = JsonSerializer.Deserialize<CouponSession>(cj);
            }

            bool isFirstItem = true;
            var now = DateTime.Now;

            foreach (var it in items)
            {
                decimal finalAmount = it.Price * it.Qty;

                if (coupon != null)
                {
                    if (coupon.type == 1) // Percentage
                    {
                        finalAmount *= coupon.value;
                    }
                    else // Fixed amount
                    {
                        if (isFirstItem)
                        {
                            finalAmount -= coupon.value;
                            isFirstItem = false;
                        }
                    }
                    finalAmount = Math.Max(0, finalAmount); // Ensure not negative
                }

                _db.CustomerOrders.Add(new Models.CustomerOrders
                {
                    CustomerID = cid,
                    ProductID = it.ProductId,
                    OrderStatusID = 2,  // 未付款
                    TotalAmount = (int)finalAmount,
                    CreateTime = now,
                    UpdateTime = now
                });
            }
            await _db.SaveChangesAsync();

            // Mark coupon as used
            if (coupon != null && coupon.id > 0)
            {
                var rec = await _db.CustomerCouponsRecords
                    .FirstOrDefaultAsync(x => x.CustomerID == cid && x.CouponID == coupon.id);
                if (rec != null)
                {
                    rec.IsUsed = true;
                    rec.UsedTime = DateTime.Now;
                    await _db.SaveChangesAsync();
                }
            }

            SaveCart(new List<CartItem>());
            // Clear coupon from session after checkout
            HttpContext.Session.Remove("CART_COUPON");
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
        public async Task<IActionResult> ApplyCoupon([FromForm] string code)
        {
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();

            code = (code ?? "").Trim();
            if (string.IsNullOrEmpty(code)) return BadRequest(new { ok = false, error = "請輸入折價碼" });

            var now = DateTime.Now;

            			var q =
            				from r in _db.CustomerCouponsRecords.Include(r => r.Coupon)
            				where r.CustomerID == cid
            					  && (r.IsUsed == null || r.IsUsed == false)
            					  && (r.Coupon != null)                              // 👈 防呆
            					  && (r.Coupon.CouponCode == code || r.Coupon.DiscountCode == code)
            				select r;
			var rec = await q.FirstOrDefaultAsync();
            if (rec == null) return BadRequest(new { ok = false, error = "此折價券不可使用" });

            // 寫入 Session（後續付款成功要把它標記已用）
            var couponObj = new
            {
                id = rec.CouponID,
                code = rec.Coupon.CouponCode,
                type = rec.Coupon.DiscountType, // "percent" or "fixed"
                value = rec.Coupon.DiscountValue,
                hint = rec.Coupon.CouponDesc
            };

            HttpContext.Session.SetString("CART_COUPON",
                System.Text.Json.JsonSerializer.Serialize(couponObj));

            return Ok(new { ok = true, hint = couponObj.hint });
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
        // 依 Session 的購物車，只針對指定 productIds 建立「未付款」訂單
        [HttpPost("checkout-by-selected")]
        public async Task<IActionResult> CheckoutBySelected([FromForm] int[] productIds)
        {
            if (productIds == null || productIds.Length == 0)
                return BadRequest(new { ok = false, error = "未指定商品" });
            var customerId = CurrentCustomerId;
            if (customerId <= 0) return Unauthorized();

            var items = GetCart();
            var selected = items.Where(x => productIds.Contains(x.ProductId)).ToList();
            if (selected.Count == 0)
                return BadRequest(new { ok = false, error = "找不到指定商品" });

            // Handle coupon logic
            CouponSession? coupon = null;
            var cj = HttpContext.Session.GetString("CART_COUPON");
            if (!string.IsNullOrWhiteSpace(cj))
            {
                coupon = JsonSerializer.Deserialize<CouponSession>(cj);
            }

            bool isFirstItem = true;
            var now = DateTime.Now;

            foreach (var it in selected)
            {
                decimal finalAmount = it.Price * it.Qty;

                if (coupon != null)
                {
                    if (coupon.type == 1) // Percentage
                    {
                        finalAmount *= coupon.value;
                    }
                    else // Fixed amount
                    {
                        if (isFirstItem)
                        {
                            finalAmount -= coupon.value;
                            isFirstItem = false;
                        }
                    }
                    finalAmount = Math.Max(0, finalAmount); // Ensure not negative
                }

                _db.CustomerOrders.Add(new CustomerOrders
                {
                    CustomerID = customerId,
                    ProductID = it.ProductId,
                    OrderStatusID = (int)OrderStatusId.Unpaid, // 未付款
                    TotalAmount = (int)finalAmount,
                    CreateTime = now,
                    UpdateTime = now
                });
            }
            await _db.SaveChangesAsync();

            // 從購物車移除已結帳（未付款）的項目
            items.RemoveAll(x => productIds.Contains(x.ProductId));
            SaveCart(items);

            // Clear coupon from session if all items that were in the cart are checked out
            if (items.Count == 0)
            {
                HttpContext.Session.Remove("CART_COUPON");
            }

            return Ok(new { ok = true });
        }
		[HttpGet("count")]
		public IActionResult GetCartCount()
		{
			var items = GetCart();
			return Ok(new { count = items.Count });
		}
	}
}
