using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Authorize(AuthenticationSchemes = "CustomerAuth")] // 跟你專案相同的 Scheme
	[Route("CustomersArea/[controller]/[action]")]
	public class FavoritesController : Controller
	{
		private readonly webtravel2Context _context;

		private const int PageSize = 9;

		public FavoritesController(webtravel2Context context)
		{
			_context = context;
		}

		// ✅ 取得目前登入會員 ID
		private int? GetCustomerId()
		{
			if (User.Identity?.IsAuthenticated == true)
			{
				return int.TryParse(User.FindFirst("CustomerID")?.Value, out int id) ? id : null;
			}
			return null;
		}

		// ✅ 收藏 / 取消收藏 API（前端 AJAX 用）
		[HttpPost]
		public async Task<IActionResult> Toggle(int productId)
		{
			var customerId = GetCustomerId();

			// ❗ 未登入 → 回傳 JSON，不做 302 轉址
			if (customerId == null)
			{
				return Unauthorized(new
				{
					ok = false,
					redirect = Url.Action("Login", "CusLogReg", new { area = "CustomersArea" })
				});
			}

			var fav = await _context.Favorites
				.FirstOrDefaultAsync(f => f.CustomerID == customerId && f.ProductID == productId);

			if (fav == null)
			{
				fav = new Favorites
				{
					CustomerID = customerId.Value,
					ProductID = productId,
					CreatedAt = DateTime.Now
				};
				_context.Favorites.Add(fav);
				await _context.SaveChangesAsync();

				int count = await _context.Favorites.CountAsync(f => f.ProductID == productId);

				return Json(new { ok = true, isFav = true, count });
			}
			else
			{
				_context.Favorites.Remove(fav);
				await _context.SaveChangesAsync();

				int count = await _context.Favorites.CountAsync(f => f.ProductID == productId);

				return Json(new { ok = true, isFav = false, count });
			}
		}

		// ✅ 收藏數量
		[HttpGet]
		public async Task<IActionResult> Count(int productId)
		{
			int count = await _context.Favorites.CountAsync(f => f.ProductID == productId);
			return Json(new { count });
		}

		// ✅ 我的收藏 ID 列表
		[HttpGet]
		public async Task<IActionResult> MyIds()
		{
			var customerId = GetCustomerId();
			if (customerId == null)
				return Unauthorized();

			var ids = await _context.Favorites
				.Where(f => f.CustomerID == customerId)
				.Select(f => f.ProductID)
				.ToListAsync();

			return Json(ids);
		}

		/// <summary>
		/// 我的收藏清單（只針對 Products）
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> List(string sort = "recent", int page = 1)
		{
			var cidStr = User.FindFirst("CustomerId")?.Value ?? User.FindFirst("CustomerID")?.Value;
			if (!int.TryParse(cidStr, out var cid))
			{
				// 未登入導到登入頁（依你專案路徑）
				return Redirect(Url.Action("Login", "CusLogReg", new
				{
					area = "CustomersArea",
					returnUrl = Request.Path + Request.QueryString
				})!);
			}

			const int PageSize = 9;

			// 先把 Products.Include(Region) 起來，避免 Region 為 null 的延遲載入問題
			var products = _context.Products
				.AsNoTracking()
				.Include(p => p.Region)   // ★ 關鍵
				.Where(p => p.IsActive == true);

			// 以 Favorites 為主去 join Products（內連接：不存在的產品會被排除）
			var q = _context.Favorites
				.AsNoTracking()
				.Where(f => f.CustomerID == cid)
				.Join(products,
					  f => f.ProductID,
					  p => p.ProductID,
					  (f, p) => new { f, p });

			// 排序
			sort = (sort ?? "recent").ToLowerInvariant();
			q = sort switch
			{
				"price_asc" => q.OrderBy(x => x.p.ProductPrice ?? 0).ThenByDescending(x => x.f.CreatedAt),
				"price_desc" => q.OrderByDescending(x => x.p.ProductPrice ?? 0).ThenByDescending(x => x.f.CreatedAt),
				"name" => q.OrderBy(x => x.p.ProductName).ThenByDescending(x => x.f.CreatedAt),
				_ => q.OrderByDescending(x => x.f.CreatedAt) // recent
			};

			// 分頁
			page = Math.Max(1, page);
			var total = await q.CountAsync();
			var rows = await q.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

			// 投影到 ViewModel（全部加上 null 防護）
			var items = rows.Select(x => new SearchResultItemVM
			{
				ProductID = x.p.ProductID,
				Name = x.p.ProductName ?? "(未命名)",
				RegionName = x.p.Region?.RegionName ?? "—",
				MinPrice = (int)(x.p.ProductPrice ?? 0),
				CoverImageUrl = string.IsNullOrWhiteSpace(x.p.ProductImageUrl) ? "/images/NoImage.png" : x.p.ProductImageUrl
			}).ToList();

			var vm = new FavoritesListVM
			{
				Sort = sort,
				Page = page,
				PageSize = PageSize,
				TotalCount = total,
				Items = items
			};

			return View(vm);
		}

	}
	// ===== 專用 ViewModel：與 SearchResultItemVM 並用 =====
	public class FavoritesListVM
	{
		public string Sort { get; set; } = "recent";
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 9;
		public int TotalCount { get; set; }
		public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
		public List<SearchResultItemVM> Items { get; set; } = new();
	}
}
