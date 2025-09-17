using Cat_Paw_Footprint.Areas.TravelManagement.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.TravelManagement.Controllers
{
    [Area("TravelManagement")]
    public class RestaurantsController : Controller
    {
        private readonly webtravel2Context _context;

        public RestaurantsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: TravelManagement/Restaurants
        public async Task<IActionResult> Index()
        {
			var restaurants = _context.Restaurants
				.Include(r => r.RestaurantPics)
				.Include(r => r.RestaurantKeywords)
				.ThenInclude(hk => hk.Keyword)
				.Include(r => r.District)
				.Include(r => r.Region);

			ViewBag.Regions = _context.Regions
				.Select(r => new SelectListItem
				{
					Value = r.RegionID.ToString(),
					Text = r.RegionName
				}).ToList();

			var viewModel = await restaurants.Select(r => new RestaurantsViewModel
			{
				RestaurantID = r.RestaurantID,
				RestaurantName = r.RestaurantName,
				RestaurantAddr = r.RestaurantAddr,
				RestaurantLat = r.RestaurantLat,
				RestaurantLng = r.RestaurantLng,
				RestaurantDesc = r.RestaurantDesc,
				DistrictID = r.District.DistrictID,
				DistrictName = r.District.DistrictName,
				District = r.District,
				RegionID = r.Region.RegionID,
				RegionName = r.Region.RegionName,
				Rating = r.Rating,
				Views = r.Views,
				RestaurantCode = r.RestaurantCode,
				IsActive = r.IsActive
			}).ToListAsync();

			return View(viewModel);
		}

        // GET: TravelManagement/Restaurants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurants = await _context.Restaurants
                .Include(r => r.RestaurantPics)
                .Include(r => r.RestaurantKeywords)
                .ThenInclude(hk => hk.Keyword)
				.Include(r => r.District)
                .Include(r => r.Region)
                .FirstOrDefaultAsync(r => r.RestaurantID == id);

			ViewBag.Regions = _context.Regions
				.Select(r => new SelectListItem
				{
					Value = r.RegionID.ToString(),
					Text = r.RegionName
				}).ToList();

			if (restaurants == null)
            {
                return NotFound();
            }

            var viewModel = new RestaurantsViewModel
            {
                RestaurantID = restaurants.RestaurantID,
                RestaurantName = restaurants.RestaurantName,
                RestaurantAddr = restaurants.RestaurantAddr,
                RestaurantLat = restaurants.RestaurantLat,
                RestaurantLng = restaurants.RestaurantLng,
                RestaurantDesc = restaurants.RestaurantDesc,
                DistrictID = restaurants.District.DistrictID,
                DistrictName = restaurants.District.DistrictName,
                District = restaurants.District,
                RegionID = restaurants.Region.RegionID,
                RegionName = restaurants.Region.RegionName,
				Region = restaurants.Region,
				Rating = restaurants.Rating,
                Views = restaurants.Views,
                RestaurantCode = restaurants.RestaurantCode,
                IsActive = restaurants.IsActive,
				KeywordID = restaurants.RestaurantKeywords.Select(k => k.Keyword.KeywordID).ToList(),
				KeywordNames = restaurants.RestaurantKeywords.Select(k => k.Keyword.Keyword).ToList(),
				PictureBase64 = restaurants.RestaurantPics.Select(p => "data:image/png;base64," + Convert.ToBase64String(p.Picture)).ToList()
			};

			return View(viewModel);
        }

        // GET: TravelManagement/Restaurants/Create
        public IActionResult Create()
        {
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName");
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName");
			ViewBag.KeywordID = new SelectList(_context.Keywords, "KeywordID", "Keyword");
			return View();
        }

        // POST: TravelManagement/Restaurants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RestaurantsViewModel model )
        {
            if (ModelState.IsValid)
            {
                var restaurants = new Restaurants
                {
                    RestaurantName = model.RestaurantName,
                    RestaurantAddr = model.RestaurantAddr,
                    RestaurantLat = model.RestaurantLat,
                    RestaurantLng = model.RestaurantLng,
                    RestaurantDesc = model.RestaurantDesc,
                    DistrictID = model.DistrictID,
                    RegionID = model.RegionID,
                    Rating = model.Rating,
                    Views = model.Views,
                    IsActive = model.IsActive,
                    RestaurantCode = model.RestaurantCode
                };
				_context.Restaurants.Add(restaurants);
				await _context.SaveChangesAsync();

				// 圖片處理				
				if (model.Picture != null && model.Picture.Any())
				{
					foreach (var file in model.Picture)
					{
						if (file.Length > 0) // 確保有檔案
						{
							using var ms = new MemoryStream();
							await file.CopyToAsync(ms);

							var pic = new RestaurantPics
							{
								RestaurantID = restaurants.RestaurantID,
								Picture = ms.ToArray()
							};

							_context.RestaurantPics.Add(pic);
						}
					}

					await _context.SaveChangesAsync(); // 一次存入所有圖片
				}

				// 關鍵字處理
				if (model.KeywordID != null)
				{
					foreach (var keywordId in model.KeywordID)
					{
						_context.RestaurantKeywords.Add(new RestaurantKeywords
						{
							RestaurantID = restaurants.RestaurantID,
							KeywordID = keywordId
						});
					}
				}
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
            }

			// 如果失敗，回傳表單內容
			ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName", model.DistrictID);
			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", model.RegionID);
			ViewData["KeywordID"] = new MultiSelectList(_context.Keywords, "KeywordID", "KeywordName", model.KeywordID);
			return View(model);
		}

        // GET: TravelManagement/Restaurants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurants = await _context.Restaurants
                .Include(r => r.RestaurantPics) // 包含圖片
				.Include(r => r.RestaurantKeywords) // 包含關鍵字
				.ThenInclude(hk => hk.Keyword)  // 包含關鍵字詳細資料
				.FirstOrDefaultAsync(r => r.RestaurantID == id);

			if (restaurants == null)
            {
                return NotFound();
            }

			// ✅ 關鍵字：一次投影，保證數量相同
			var keywordList = restaurants.RestaurantKeywords
				.Where(k => k.Keyword != null && k.KeywordID != null)
				.Select(k => new { Id = k.KeywordID.Value, Name = k.Keyword.Keyword })
				.ToList();

			// 塞進 ViewModel
			var viewModel = new RestaurantsViewModel
            {
                RestaurantID = restaurants.RestaurantID,
                RestaurantName = restaurants.RestaurantName,
                RestaurantAddr = restaurants.RestaurantAddr,
                RestaurantLat = restaurants.RestaurantLat,
                RestaurantLng = restaurants.RestaurantLng,
                RestaurantDesc = restaurants.RestaurantDesc,
                DistrictID = restaurants.DistrictID,
                RegionID = restaurants.RegionID,
                Rating = restaurants.Rating,
                Views = restaurants.Views,
                RestaurantCode = restaurants.RestaurantCode,
                IsActive = restaurants.IsActive,

				// 關鍵字
				KeywordID = restaurants.RestaurantKeywords.Select(k => k.KeywordID ?? 0).ToList(),
				KeywordNames = restaurants.RestaurantKeywords.Select(k => k.Keyword.Keyword).ToList(),

				// 圖片
				PictureIds = restaurants.RestaurantPics.Select(p => p.RestaurantPicID).ToList(),
				PictureBase64 = restaurants.RestaurantPics
					   .Select(p => "data:image/png;base64," + Convert.ToBase64String(p.Picture))
					   .ToList()
			};

			// 下拉選單 / 多選清單
			ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName", restaurants.DistrictID);
			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", restaurants.RegionID);
			ViewBag.Keywords = new MultiSelectList(_context.Keywords, "KeywordID", "Keyword", viewModel.KeywordID);

			return View(viewModel);
		}

        // POST: TravelManagement/Restaurants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,RestaurantsViewModel model )
        {
            if (id != model.RestaurantID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var restaurants = await _context.Restaurants
                    .Include(r => r.RestaurantPics)
                    .Include(r => r.RestaurantKeywords)
                    .FirstOrDefaultAsync(r => r.RestaurantID == id);

                if (restaurants == null) return NotFound();

				// ---- 更新主表資料 ----
                restaurants.RestaurantName = model.RestaurantName;
                restaurants.RestaurantAddr = model.RestaurantAddr;
                restaurants.RestaurantLat = model.RestaurantLat;
                restaurants.RestaurantLng = model.RestaurantLng;
                restaurants.RestaurantDesc = model.RestaurantDesc;
                restaurants.DistrictID = model.DistrictID;
                restaurants.RegionID = model.RegionID;
                restaurants.Rating = model.Rating;
                restaurants.Views = model.Views;
                restaurants.IsActive = model.IsActive;
                restaurants.RestaurantCode = model.RestaurantCode;

				// ---- 刪除舊圖片（等按確認才真正刪 DB）----
				if (model.DeletedPictureIds != null && model.DeletedPictureIds.Any())
				{
					var picsToDelete = _context.RestaurantPics
						.Where(p => model.DeletedPictureIds.Contains(p.RestaurantPicID))
						.ToList();

					_context.RestaurantPics.RemoveRange(picsToDelete);
				}

				// ---- 新增新圖片 ----
				if (model.Picture != null && model.Picture.Any())
				{
					foreach (var file in model.Picture)
					{
						if (file.Length > 0)
						{
							using var ms = new MemoryStream();
							await file.CopyToAsync(ms);

							var pic = new RestaurantPics
							{
								RestaurantID = restaurants.RestaurantID,
								Picture = ms.ToArray()
							};
							_context.RestaurantPics.Add(pic);
						}
					}
					await _context.SaveChangesAsync();// 一次存入所有圖片
				}

				// ---- 更新關鍵字 ---- (先清空再加新的)
				restaurants.RestaurantKeywords.Clear();
				if (model.KeywordID != null && model.KeywordID.Any())
				{
					foreach (var keywordId in model.KeywordID)
					{
						restaurants.RestaurantKeywords.Add(new RestaurantKeywords
						{
							RestaurantID = restaurants.RestaurantID,
							KeywordID = keywordId
						});
					}
				}
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));

			}
			// 驗證失敗回傳畫面
			ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName", model.DistrictID);
			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", model.RegionID);
			ViewBag.Keywords = new MultiSelectList(_context.Keywords, "Keyword", "Keyword", model.KeywordID);

			return View(model);
		}

		// ---- 單張刪除圖片 API (AJAX 用) ----
		[HttpPost]
		public async Task<IActionResult> DeletePicture(int id)
		{
			var pic = await _context.RestaurantPics.FindAsync(id);
			if (pic == null) return NotFound();

			_context.RestaurantPics.Remove(pic);
			await _context.SaveChangesAsync();

			return Ok(); // 讓前端 JS 判斷成功
		}

		// GET: TravelManagement/Restaurants/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurants = await _context.Restaurants
                .Include(r => r.District)
                .Include(r => r.Region)
                .FirstOrDefaultAsync(m => m.RestaurantID == id);
            if (restaurants == null)
            {
                return NotFound();
            }

            return View(restaurants);
        }

        // POST: TravelManagement/Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurants = await _context.Restaurants.FindAsync(id);
            if (restaurants != null)
            {
                _context.Restaurants.Remove(restaurants);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantsExists(int id)
        {
            return _context.Restaurants.Any(e => e.RestaurantID == id);
        }
    }
}
