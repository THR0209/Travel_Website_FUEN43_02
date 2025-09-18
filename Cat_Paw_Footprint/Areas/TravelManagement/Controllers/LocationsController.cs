using Cat_Paw_Footprint.Areas.TravelManagement.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
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
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaTravelManagement")]
	public class LocationsController : Controller
    {
        private readonly webtravel2Context _context;

        public LocationsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: TravelManagement/Locations
        public async Task<IActionResult> Index()
        {
			var location = _context.Locations
				.Include(l => l.LocationPics)
				.Include(l => l.LocationKeywords)
				.ThenInclude(hk => hk.Keyword)
				.Include(l => l.District)
				.Include(l => l.Region);

			ViewBag.Regions = _context.Regions
				.Select(l => new SelectListItem
				{
					Value = l.RegionID.ToString(),
					Text = l.RegionName
				}).ToList();

			var viewModel = await location.Select(l => new LocationsViewModel
			{
				LocationID = l.LocationID,
				LocationName = l.LocationName,
				LocationAddr = l.LocationAddr,
				LocationLat = l.LocationLat,
				LocationLng = l.LocationLng,
				LocationDesc = l.LocationDesc,
				LocationPrice = l.LocationPrice,
				DistrictID = l.District.DistrictID,
				DistrictName = l.District.DistrictName,
				District = l.District,
				RegionID = l.Region.RegionID,
				RegionName = l.Region.RegionName,
				Rating = l.Rating,				
				Views = l.Views,
				LocationCode = l.LocationCode,
				IsActive = l.IsActive				
			}).ToListAsync();

			 return View(viewModel);
        }

        // GET: TravelManagement/Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locations = await _context.Locations
				.Include(l => l.LocationPics)
				.Include(l => l.LocationKeywords)
				.ThenInclude(hk => hk.Keyword)
				.Include(l => l.District)
                .Include(l => l.Region)
                .FirstOrDefaultAsync(l => l.LocationID == id);

			ViewBag.Regions = _context.Regions
				.Select(l => new SelectListItem
				{
					Value = l.RegionID.ToString(),
					Text = l.RegionName
				}).ToList();

			if (locations == null)
            {
                return NotFound();
            }

			var viewModel = new LocationsViewModel
			{
				LocationID = locations.LocationID,
				LocationName = locations.LocationName,
				LocationAddr = locations.LocationAddr,
				LocationLat = locations.LocationLat,
				LocationLng = locations.LocationLng,
				LocationDesc = locations.LocationDesc,
				LocationPrice = locations.LocationPrice,
				DistrictID = locations.DistrictID,
				DistrictName = locations.District.DistrictName,
				District = locations.District,
				RegionID = locations.RegionID,
				RegionName = locations.Region.RegionName,
				Region = locations.Region,
				Rating = locations.Rating,
				Views = locations.Views,
				LocationCode = locations.LocationCode,
				IsActive = locations.IsActive,
				KeywordID = locations.LocationKeywords.Select(k => k.Keyword.KeywordID).ToList(),
				KeywordNames = locations.LocationKeywords.Select(k => k.Keyword.Keyword).ToList(),
				PictureBase64 = locations.LocationPics.Select(p => "data:image/png;base64," + Convert.ToBase64String(p.Picture)).ToList()
			};

			return View(viewModel);
        }

        // GET: TravelManagement/Locations/Create
        public IActionResult Create()
        {
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName");
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName");
			ViewBag.KeywordID = new SelectList(_context.Keywords, "KeywordID", "Keyword");
			return View();
        }

        // POST: TravelManagement/Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var location = new Locations
                { 
                    LocationName = model.LocationName,
                    LocationAddr = model.LocationAddr,
                    LocationLat = model.LocationLat,
                    LocationLng = model.LocationLng,
                    LocationDesc = model.LocationDesc,
                    LocationPrice = model.LocationPrice,
                    RegionID = model.RegionID,
                    DistrictID = model.DistrictID,
                    Rating = model.Rating,
                    Views = model.Views,
                    LocationCode = model.LocationCode,
                    IsActive = model.IsActive
                };	
			    _context.Add(location);
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

							var pic = new LocationPics
							{
								LocationID = location.LocationID,
								Picture = ms.ToArray()
							};

							_context.LocationPics.Add(pic);
						}
					}

					await _context.SaveChangesAsync(); // 一次存入所有圖片
				}

				// 關鍵字處理
				if (model.KeywordID != null)
				{
					foreach (var keywordId in model.KeywordID)
					{
						_context.LocationKeywords.Add(new LocationKeywords
						{
							LocationID = location.LocationID,
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

        // GET: TravelManagement/Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locations = await _context.Locations
                .Include(l => l.LocationPics)   // 包含圖片
				.Include(l => l.LocationKeywords)   // 包含關鍵字
				.ThenInclude(hk => hk.Keyword)  // 包含關鍵字詳細資料
				.FirstOrDefaultAsync(l => l.LocationID == id);

			if (locations == null)
            {
                return NotFound();
            }

			// ✅ 關鍵字：一次投影，保證數量相同
			var keywordList = locations.LocationKeywords
				.Where(k => k.Keyword != null && k.KeywordID != null)
				.Select(k => new { Id = k.KeywordID.Value, Name = k.Keyword.Keyword })
				.ToList();

            // 塞進 ViewModel
            var viewModel = new LocationsViewModel
            {
                LocationID = locations.LocationID,
                LocationName = locations.LocationName,
                LocationAddr = locations.LocationAddr,
                LocationLat = locations.LocationLat,
                LocationLng = locations.LocationLng,
                LocationDesc = locations.LocationDesc,
                LocationPrice = locations.LocationPrice,
                RegionID = locations.RegionID,
                DistrictID = locations.DistrictID,
                Rating = locations.Rating,
                Views = locations.Views,
                LocationCode = locations.LocationCode,
                IsActive = locations.IsActive,

				// 關鍵字
				KeywordID = locations.LocationKeywords.Select(k => k.KeywordID ?? 0).ToList(),
				KeywordNames = locations.LocationKeywords.Select(k => k.Keyword.Keyword).ToList(),

				// 圖片
				PictureIds = locations.LocationPics.Select(p => p.LocationPicID).ToList(),
				PictureBase64 = locations.LocationPics
					   .Select(p => "data:image/png;base64," + Convert.ToBase64String(p.Picture))
					   .ToList()
			};

			// 下拉選單 / 多選清單
			ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName", locations.DistrictID);
			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", locations.RegionID);
			ViewBag.Keywords = new MultiSelectList(_context.Keywords, "KeywordID", "Keyword", viewModel.KeywordID);

			return View(viewModel);
		}

        // POST: TravelManagement/Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LocationsViewModel model )
        {
            if (id != model.LocationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var locations = await _context.Locations
                    .Include(l => l.LocationPics)
                    .Include(l => l.LocationKeywords)
                    .FirstOrDefaultAsync(l => l.LocationID == id);

				if (locations == null) return NotFound();

				// ---- 更新主表資料 ----
                locations.LocationName = model.LocationName;
                locations.LocationAddr = model.LocationAddr;
                locations.LocationLat = model.LocationLat;
                locations.LocationLng = model.LocationLng;
                locations.LocationDesc = model.LocationDesc;
                locations.LocationPrice = model.LocationPrice;
                locations.RegionID = model.RegionID;
                locations.DistrictID = model.DistrictID;
                locations.Rating = model.Rating;
                locations.Views = model.Views;
                locations.LocationCode = model.LocationCode;
                locations.IsActive = model.IsActive;

				// ---- 刪除舊圖片（等按確認才真正刪 DB）----
				if (model.DeletedPictureIds != null && model.DeletedPictureIds.Any())
				{
					var picsToDelete = _context.LocationPics
						.Where(p => model.DeletedPictureIds.Contains(p.LocationPicID))
						.ToList();

					_context.LocationPics.RemoveRange(picsToDelete);
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

							var pic = new LocationPics
							{
								LocationID = locations.LocationID,
								Picture = ms.ToArray()
							};
							_context.LocationPics.Add(pic);
						}
					}
					await _context.SaveChangesAsync();// 一次存入所有圖片
				}

				// ---- 更新關鍵字 ---- (先清空再加新的)
				locations.LocationKeywords.Clear();
				if (model.KeywordID != null && model.KeywordID.Any())
				{
					foreach (var keywordId in model.KeywordID)
					{
						locations.LocationKeywords.Add(new LocationKeywords
						{
							LocationID = locations.LocationID,
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
			var pic = await _context.LocationPics.FindAsync(id);
			if (pic == null) return NotFound();

			_context.LocationPics.Remove(pic);
			await _context.SaveChangesAsync();

			return Ok(); // 讓前端 JS 判斷成功
		}

		// GET: TravelManagement/Locations/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locations = await _context.Locations
                .Include(l => l.District)
                .Include(l => l.Region)
                .FirstOrDefaultAsync(m => m.LocationID == id);
            if (locations == null)
            {
                return NotFound();
            }

            return View(locations);
        }

        // POST: TravelManagement/Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var locations = await _context.Locations.FindAsync(id);
            if (locations != null)
            {
                _context.Locations.Remove(locations);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationsExists(int id)
        {
            return _context.Locations.Any(e => e.LocationID == id);
        }
    }
}
