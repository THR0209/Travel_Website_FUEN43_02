using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	public class ProductsController : Controller
	{
		private readonly webtravel2Context _context;
		private const int PageSize = 9;

		public ProductsController(webtravel2Context context)
		{
			_context = context;
		}

		// GET: /CustomersArea/Products/Search
		// Areas/CustomersArea/Controllers/ProductsController.cs

		[HttpGet]
		public async Task<IActionResult> Search([FromQuery] SearchInput input, string sort = "pop", int page = 1)
		{
			input ??= new SearchInput();
			input.Type = string.IsNullOrWhiteSpace(input.Type) ? "trip" : input.Type.Trim().ToLower();

			bool isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

			IQueryable<SearchResultItemVM> q;

			// === 資料來源 ===
			if (input.Type == "trip")
			{
				q = _context.Products
					.AsNoTracking()
					.Where(p => p.IsActive == true)
					.Select(p => new SearchResultItemVM
					{
						ProductID = p.ProductID,
						Name = p.ProductName,
						RegionName = p.Region.RegionName,
						MinPrice = (int)(p.ProductPrice ?? 0),
						CoverImageUrl = string.IsNullOrEmpty(p.ProductImageUrl) ? "/images/NoImage.png" : p.ProductImageUrl,
						// for keyword exact-match
						Keywords = p.ProductsKeywords.Select(pk => pk.Keyword.Keyword).ToList()
					});
			}
			else
			{
				int productType = input.Type switch
				{
					"hotel" => 1,
					"ticket" => 2,
					"car" => 3,
					_ => 0
				};

				q = _context.SemiSelfProducts
					.AsNoTracking()
					.Where(p => p.IsActive == true && (productType == 0 || p.ProductType == productType))
					.Select(p => new SearchResultItemVM
					{
						ProductID = p.ProductID,
						Name = p.ProductName,
						RegionName = p.Region.RegionName,
						MinPrice = (int)(p.ProductPrice ?? 0),
						CoverImageUrl = string.IsNullOrEmpty(p.ProductImageUrl) ? "/images/NoImage.png" : p.ProductImageUrl,
						Keywords = p.SemiKeywords.Select(sk => sk.Keyword.Keyword).ToList()
					});
			}

			// === 目的地/關鍵字（輸入框）===
			if (!string.IsNullOrWhiteSpace(input.Destination))
			{
				var key = input.Destination.Trim();
				q = q.Where(x =>
					EF.Functions.Like(x.Name, $"%{key}%") ||          // 名稱模糊
					EF.Functions.Like(x.RegionName, $"%{key}%") ||    // 區域模糊
					x.Keywords.Any(k => k == key));                   // 關鍵字完全相等
			}

			// === 出發地（checkbox 多選）：名稱模糊 + 關鍵字完全相等 ===
			if (input.Departures != null && input.Departures.Any())
			{
				var deps = input.Departures;
				q = q.Where(x =>
					deps.Any(dep => x.Name.Contains(dep)) ||          // 名稱包含任一出發地
					x.Keywords.Any(k => deps.Contains(k)));           // 關鍵字完全相等
			}

			// === 價格 ===
			if (input.MinPrice.HasValue) q = q.Where(x => x.MinPrice >= input.MinPrice.Value);
			if (input.MaxPrice.HasValue) q = q.Where(x => x.MinPrice <= input.MaxPrice.Value);

			// === 排序 ===
			sort = (sort ?? "pop").ToLower();
			q = sort switch
			{
				"price_asc" => q.OrderBy(x => x.MinPrice),
				"price_desc" => q.OrderByDescending(x => x.MinPrice),
				"name" => q.OrderBy(x => x.Name),
				_ => q.OrderByDescending(x => x.ProductID)
			};

			// === 分頁 ===
			const int pageSize = 9;
			page = Math.Max(1, page);
			var total = await q.CountAsync();
			var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

			var vm = new SearchResultsVM
			{
				Input = input,
				Sort = sort,
				Page = page,
				PageSize = pageSize,
				TotalCount = total,
				Items = items
			};

			// AJAX：回傳 header + list wrapper
			if (isAjax)
				return PartialView("_SearchResultsWrapper", vm);

			// 首次載入：整頁
			return View("Search", vm);
		}



		// ✅ 查看所有：行程/住宿/門票/交通（不用目的地/日期），可篩選/排序/分頁
		// ✅ 完整替換原 Browse action
		[HttpGet]
		public async Task<IActionResult> Browse([FromQuery] SearchInput input, string sort = "pop", int page = 1)
		{
			input ??= new SearchInput();
			input.Type = string.IsNullOrWhiteSpace(input.Type) ? "trip" : input.Type.Trim().ToLower();

			bool isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

			IQueryable<SearchResultItemVM> q;

			// === 資料來源 ===
			if (input.Type == "trip")
			{
				q = _context.Products
					.AsNoTracking()
					.Where(p => p.IsActive == true)
					.Select(p => new SearchResultItemVM
					{
						ProductID = p.ProductID,
						Name = p.ProductName,
						RegionName = p.Region.RegionName,
						MinPrice = (int)(p.ProductPrice ?? 0),
						CoverImageUrl = string.IsNullOrEmpty(p.ProductImageUrl) ? "/images/NoImage.png" : p.ProductImageUrl,

						// 🔑 為了篩選關鍵字
						Keywords = p.ProductsKeywords.Select(pk => pk.Keyword.Keyword).ToList()
					});
			}
			else
			{
				int productType = input.Type switch
				{
					"hotel" => 1,
					"ticket" => 2,
					"car" => 3,
					_ => 0
				};

				q = _context.SemiSelfProducts
					.AsNoTracking()
					.Where(p => p.IsActive == true && (productType == 0 || p.ProductType == productType))
					.Select(p => new SearchResultItemVM
					{
						ProductID = p.ProductID,
						Name = p.ProductName,
						RegionName = p.Region.RegionName,
						MinPrice = (int)(p.ProductPrice ?? 0),
						CoverImageUrl = string.IsNullOrEmpty(p.ProductImageUrl) ? "/images/NoImage.png" : p.ProductImageUrl,

						Keywords = p.SemiKeywords.Select(sk => sk.Keyword.Keyword).ToList()
					});
			}

			// === 🔎 目的地/關鍵字 模糊搜尋 ===
			if (!string.IsNullOrWhiteSpace(input.Destination))
			{
				var key = input.Destination.Trim();
				q = q.Where(x =>
					EF.Functions.Like(x.Name, $"%{key}%") ||             // 名稱模糊搜尋
					EF.Functions.Like(x.RegionName, $"%{key}%") ||       // 區域模糊搜尋
					x.Keywords.Any(k => k == key));                      // 關鍵字完全相等
			}

			// === 🧭 出發地篩選 ===
			if (input.Departures != null && input.Departures.Any())
			{
				var deps = input.Departures;
				q = q.Where(x =>
					deps.Any(dep => x.Name.Contains(dep)) ||              // 名稱包含任一出發地（模糊）
					x.Keywords.Any(k => deps.Contains(k)));              // 關鍵字完全相等
			}

			// === 💰 價格篩選 ===
			if (input.MinPrice.HasValue) q = q.Where(x => x.MinPrice >= input.MinPrice.Value);
			if (input.MaxPrice.HasValue) q = q.Where(x => x.MinPrice <= input.MaxPrice.Value);

			// === 📊 排序 ===
			sort = (sort ?? "pop").ToLower();
			q = sort switch
			{
				"price_asc" => q.OrderBy(x => x.MinPrice),
				"price_desc" => q.OrderByDescending(x => x.MinPrice),
				"name" => q.OrderBy(x => x.Name),
				_ => q.OrderByDescending(x => x.ProductID)
			};

			// === 📄 分頁 ===
			const int pageSize = 9;
			page = Math.Max(1, page);
			var total = await q.CountAsync();
			var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

			var vm = new SearchResultsVM
			{
				Input = input,
				Sort = sort,
				Page = page,
				PageSize = pageSize,
				TotalCount = total,
				Items = items
			};

			// === 📤 AJAX → 回傳 partial ===
			if (isAjax)
				return PartialView("_BrowseResultsWrapper", vm);

			// === 📃 首次載入 ===
			return View("Browse", vm);
		}



		// GET: /CustomersArea/Products/Details/5
		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			// 產品本體
			var p = await _context.Products
				.AsNoTracking()
				.Where(x => x.ProductID == id && x.IsActive == true)
				.Select(x => new
				{
					x.ProductID,
					x.ProductName,
					x.Region.RegionName,
					x.ProductPrice,
					//Rating = x.Rating ?? 0,
					//x.DurationText,
					//x.Category,
					x.ProductImageUrl,                // 可能為 null
					x.StartDate,
					x.EndDate,
					DescriptionHtml = x.ProductDesc,   // 若無此欄位請改對應欄位
					NoteHtml = x.ProductNote                 // 若無此欄位請改對應欄位
				})
				.FirstOrDefaultAsync();

			if (p == null) return NotFound();

			// fallback 封面
			var cover = string.IsNullOrWhiteSpace(p.ProductImageUrl) ? "/images/NoImage.png" : p.ProductImageUrl;

			var vm = new ProductDetailVM
			{
				ProductID = p.ProductID,
				ProductName = p.ProductName,
				RegionName = p.RegionName,
				ProductPrice = (int)p.ProductPrice,
				//Rating = p.Rating,
				//DurationText = p.DurationText,
				//Category = p.Category,
				CoverImageUrl = cover,
				//ReleaseDate = p.ReleaseDate,
				//RemovalDate = p.RemovalDate,
				DescriptionHtml = p.DescriptionHtml,
				NoteHtml = p.NoteHtml,
			};

			// Locations (group by DayNumber)
			var pl = await _context.Products_Locations
				.AsNoTracking()
				.Where(x => x.ProductID == id)
				.Include(x => x.Location).ThenInclude(l => l.LocationPics)
				.OrderBy(x => x.OrderIndex)
				.ToListAsync();

			vm.LocationsByDay = pl
				.GroupBy(x => x.DayNumber)
				.OrderBy(g => g.Key)
				.Select(g => new ProductDetailVM.DayLocationsDto
				{
					DayNumber = (int)g.Key,
					Items = g.Select(x => new ProductDetailVM.LocationDto
					{
						Name = x.Location?.LocationName,
						Desc = x.Location?.LocationDesc,
						Pictures = (x.Location?.LocationPics?.Select(pic => pic.PictureUrl).Where(u => !string.IsNullOrWhiteSpace(u)).ToList()
								   ?? new()) // 若你的圖是 byte[]，請改為對應的輸出端點
					}).ToList()
				}).ToList();

			// Restaurants (group by OrderIndex = day ; Breakfast/Lunch/Dinner)
			var pr = await _context.Products_Restaurants
				.AsNoTracking()
				.Where(x => x.ProductID == id)
				.Include(x => x.Restaurant).ThenInclude(r => r.RestaurantPics)
				.OrderBy(x => x.OrderIndex)
				.ToListAsync();

			vm.MealsByDay = pr
				.GroupBy(x => x.OrderIndex)
				.OrderBy(g => g.Key)
				.Select(g => new ProductDetailVM.DayMealsDto
				{
					DayNumber = (int)g.Key,
					Meals = new[]
					{
						"Breakfast","Lunch","Dinner"
					}.Select(mt => new ProductDetailVM.MealGroup
					{
						MealType = mt,
						Items = g.Where(x => x.MealType == mt).Select(x => new ProductDetailVM.MealItem
						{
							Name = x.Restaurant?.RestaurantName,
							Desc = x.Restaurant?.RestaurantDesc,
							Pictures = (x.Restaurant?.RestaurantPics?.Select(pic => pic.PictureUrl).Where(u => !string.IsNullOrWhiteSpace(u)).ToList()
									   ?? new())
						}).ToList()
					}).ToList()
				}).ToList();

			// Hotels (group by day)
			var ph = await _context.Products_Hotels
				.AsNoTracking()
				.Where(x => x.ProductID == id)
				.Include(x => x.Hotel).ThenInclude(h => h.HotelPics)
				.OrderBy(x => x.OrderIndex)
				.ToListAsync();

			vm.HotelsByDay = ph
				.GroupBy(x => x.OrderIndex)
				.OrderBy(g => g.Key)
				.Select(g => new ProductDetailVM.DayHotelsDto
				{
					DayNumber = (int)g.Key,
					Items = g.Select(x => new ProductDetailVM.HotelItem
					{
						Name = x.Hotel?.HotelName,
						Addr = x.Hotel?.HotelAddr,
						Desc = x.Hotel?.HotelDesc,
						Pictures = (x.Hotel?.HotelPics?.Select(pic => pic.PictureUrl).Where(u => !string.IsNullOrWhiteSpace(u)).ToList()
								   ?? new())
					}).ToList()
				}).ToList();

			// Transports
			var pt = await _context.Products_Transportations
				.AsNoTracking()
				.Where(x => x.ProductID == id)
				.Include(x => x.Transport).ThenInclude(t => t.TransportPics)
				.ToListAsync();

			vm.Transports = pt.Select(x => new ProductDetailVM.TransportDto
			{
				Name = x.Transport?.TransportName,
				Desc = x.Transport?.TransportDesc,
				Pictures = (x.Transport?.TransportPics?.Select(pic => pic.PictureUrl).Where(u => !string.IsNullOrWhiteSpace(u)).ToList()
						   ?? new())
			}).ToList();

			return View(vm);
		}

		// GET: /CustomersArea/Products/GetHotProducts
		[HttpGet]
		public async Task<IActionResult> GetHotProducts()
		{
			var hotProducts = await _context.Products
				.AsNoTracking()
				.Where(p => p.IsActive == true)
				.OrderByDescending(p => p.Views) // 熱門排序依照瀏覽數，可改成銷量或評價
				.Take(8)
				.Select(p => new
				{
					p.ProductID,
					p.ProductName,
					Region = p.Region.RegionName,
					Price = p.ProductPrice ?? 0,
					Cover = string.IsNullOrEmpty(p.ProductImageUrl) ? "/images/NoImage.png" : p.ProductImageUrl
				})
				.ToListAsync();

			return Json(hotProducts);
		}


		// GET: /CustomersArea/Products/GetFeaturedHotels
		[HttpGet]
		public async Task<IActionResult> GetFeaturedHotels()
		{
			var list = await _context.SemiSelfProducts
				.AsNoTracking()
				.Where(x => x.IsActive == true && x.ProductType == 1)
				.OrderByDescending(x => x.Views)
				.Take(8)
				.Select(x => new {
					productID = x.ProductID,
					productName = x.ProductName,
					region = x.Region.RegionName,
					price = x.ProductPrice ?? 0,
					cover = string.IsNullOrEmpty(x.ProductImageUrl) ? "/images/NoImage.png" : x.ProductImageUrl
				}).ToListAsync();

			return Json(list);
		}

		// GET: /CustomersArea/Products/GetFeaturedTickets
		[HttpGet]
		public async Task<IActionResult> GetFeaturedTickets()
		{
			var list = await _context.SemiSelfProducts
				.AsNoTracking()
				.Where(x => x.IsActive == true && x.ProductType == 2)
				.OrderByDescending(x => x.Views)
				.Take(8)
				.Select(x => new {
					productID = x.ProductID,
					productName = x.ProductName,
					region = x.Region.RegionName,
					price = x.ProductPrice ?? 0,
					cover = string.IsNullOrEmpty(x.ProductImageUrl) ? "/images/NoImage.png" : x.ProductImageUrl
				}).ToListAsync();

			return Json(list);
		}

		// GET: /CustomersArea/Products/GetFeaturedTransports
		[HttpGet]
		public async Task<IActionResult> GetFeaturedTransports()
		{
			var list = await _context.SemiSelfProducts
				.AsNoTracking()
				.Where(x => x.IsActive == true && x.ProductType == 3)
				.OrderByDescending(x => x.Views)
				.Take(8)
				.Select(x => new {
					productID = x.ProductID,
					productName = x.ProductName,
					region = x.Region.RegionName,
					price = x.ProductPrice ?? 0,
					cover = string.IsNullOrEmpty(x.ProductImageUrl) ? "/images/NoImage.png" : x.ProductImageUrl
				}).ToListAsync();

			return Json(list);
		}

	}
}
