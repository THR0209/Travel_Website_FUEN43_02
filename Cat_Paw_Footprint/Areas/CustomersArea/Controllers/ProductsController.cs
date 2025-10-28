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
		private const int PageSize = 12;

		public ProductsController(webtravel2Context context)
		{
			_context = context;
		}

		// GET: /CustomersArea/Products/Search
		[HttpGet]
		public async Task<IActionResult> Search([FromQuery] SearchInput input, int page = 1, string sort = "pop")
		{
			// 基本防呆（即使前端驗證，也要後端把關）
			if (string.IsNullOrWhiteSpace(input.Destination) || input.Start == null || input.End == null)
			{
				TempData["SwalWarning"] = "請選擇「目的地」與「旅行日期」。";
				return RedirectToAction("Index", "Home", new { area = "CustomersArea" });
			}

			var startDate = input.Start.Value.Date;
			var endDate = input.End.Value.Date;

			// 基礎查詢：已上架 + 與日期有交集（ReleaseDate~RemovalDate 內）
			var query = _context.Products
				.AsNoTracking()
				.Where(p => p.IsActive == true);
			//.Where(p =>
			//	(p.StartDate == null || p.StartDate.Value.Date <= endDate) &&
			//	(p.RemovalDate == null || p.RemovalDate.Value.Date >= startDate)
			//);

			// 目的地（用 RegionName / Name 模糊比對；依你的 schema 調整）
			var key = input.Destination.Trim();
			query = query.Where(p =>
				//EF.Functions.Like(p.RegionName, $"%{key}%") ||
				EF.Functions.Like(p.ProductName, $"%{key}%")
			);

			// 類別（依你的欄位調整）
			if (!string.IsNullOrWhiteSpace(input.Type))
			{
				switch (input.Type)
				{
					//case "trip": query = query.Where(p => p.Category == "Trip" || p.Category == "Tour"); break;
					//case "hotel": query = query.Where(p => p.Category == "Hotel"); break;
					//case "ticket": query = query.Where(p => p.Category == "Ticket" || p.Category == "Attraction"); break;
					//case "car": query = query.Where(p => p.Category == "Car" || p.Category == "Transport"); break;
				}
			}

			// 旅客數（若你有容量欄位可加；沒有可略）
			if (input.People > 0)
			{
				// e.g. if (p.Capacity != null) query = query.Where(p => p.Capacity >= input.People);
			}

			// 側邊篩選
			if (input.MinPrice.HasValue) query = query.Where(p => (p.ProductPrice ?? 0) >= input.MinPrice.Value);
			if (input.MaxPrice.HasValue) query = query.Where(p => (p.ProductPrice ?? 0) <= input.MaxPrice.Value);
			//if (input.MinRating.HasValue) query = query.Where(p => (p.Rating ?? 0) >= input.MinRating.Value);
			// 若有天數欄位可追加：
			// if (input.MinDays.HasValue) query = query.Where(p => (p.DurationDays ?? 0) >= input.MinDays.Value);
			// if (input.MaxDays.HasValue) query = query.Where(p => (p.DurationDays ?? 0) <= input.MaxDays.Value);

			// 排序
			query = sort switch
			{
				"price_asc" => query.OrderBy(p => p.ProductPrice),
				"price_desc" => query.OrderByDescending(p => p.ProductPrice),
				//"rating" => query.OrderByDescending(p => p.Rating),
				_ => query.OrderByDescending(p => p.Views).ThenByDescending(p => p.StartDate) // 依時間排序的部分可能要更改
			};

			// 總數 + 分頁 + 投影
			var total = await query.CountAsync();
			var items = await query
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.Select(p => new SearchResultItemVM
				{
					ProductID = p.ProductID,
					Name = p.ProductName,
					RegionName = p.Region.RegionName,
					MinPrice = p.ProductPrice ?? 0,
					//Rating = p.Rating ?? 0,
					//DurationText = p.DurationText,
					CoverImageUrl = string.IsNullOrEmpty(p.ProductImageUrl) ? "/images/NoImage.png" : p.ProductImageUrl
				})
				.ToListAsync();

			var vm = new SearchResultsVM
			{
				Input = input,
				Sort = sort,
				Page = page,
				PageSize = PageSize,
				TotalCount = total,
				Items = items
			};

			// AJAX：只回傳列表 Partial
			var isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
			if (isAjax) return PartialView("_SearchResultsList", vm);

			return View(vm);
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
	}
}
