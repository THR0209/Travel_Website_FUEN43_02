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
    public class HotelsController : Controller
    {
        private readonly webtravel2Context _context;

        public HotelsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: TravelManagement/Hotels
        public async Task<IActionResult> Index()
        {
            var hotels = _context.Hotels
                .Include(h => h.HotelPics)
                .Include(h => h.HotelKeywords)
                .ThenInclude(hk => hk.Keyword)
                .Include(h => h.District)
                .Include(h => h.Region);

            ViewBag.Regions = _context.Regions
                .Select(r => new SelectListItem
                {
                    Value = r.RegionID.ToString(),
                    Text = r.RegionName
                }).ToList();

			var viewModel = await hotels.Select(h => new HotelsViewModel
            {
                HotelID = h.HotelID,
                HotelName = h.HotelName,
                HotelAddr = h.HotelAddr,
                HotelLat = h.HotelLat,
                HotelLng = h.HotelLng,
                HotelDesc = h.HotelDesc,
                DistrictID = h.District.DistrictID,
                DistrictName = h.District.DistrictName,
				District = h.District,
				RegionID = h.Region.RegionID,
				RegionName = h.Region.RegionName,
				Rating = h.Rating,
                Views = h.Views,
                HotelCode = h.HotelCode,
                IsActive = h.IsActive
            }).ToListAsync();
            
			return View(viewModel);
        }

        // GET: TravelManagement/Hotels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var hotel = await _context.Hotels
				.Include(h => h.HotelPics)
				.Include(h => h.HotelKeywords)
				.ThenInclude(hk => hk.Keyword)
				.Include(h => h.District)
				.Include(h => h.Region)
				.FirstOrDefaultAsync(h => h.HotelID == id);

			ViewBag.Regions = _context.Regions
				.Select(r => new SelectListItem
				{
					Value = r.RegionID.ToString(),
					Text = r.RegionName
				}).ToList();

			
			if (hotel == null)
            {
                return NotFound();
            }

			var viewModel = new HotelsViewModel
			{
				HotelID = hotel.HotelID,
				HotelName = hotel.HotelName,
				HotelAddr = hotel.HotelAddr,
				HotelLat = hotel.HotelLat,
				HotelLng = hotel.HotelLng,
				HotelDesc = hotel.HotelDesc,
				DistrictID = hotel.DistrictID,
				DistrictName = hotel.District.DistrictName,
				District = hotel.District,
				RegionID = hotel.RegionID,
				RegionName = hotel.Region.RegionName,
				Region = hotel.Region,
				Rating = hotel.Rating,
				Views = hotel.Views,
				HotelCode = hotel.HotelCode,
				IsActive = hotel.IsActive,
				KeywordID = hotel.HotelKeywords.Select(k => k.Keyword.KeywordID).ToList(),
				KeywordNames = hotel.HotelKeywords.Select(k => k.Keyword.Keyword).ToList(),
				PictureBase64 = hotel.HotelPics.Select(p => "data:image/png;base64," + Convert.ToBase64String(p.Picture)).ToList()
			};

			return View(viewModel);
		}

        // GET: TravelManagement/Hotels/Create
        public IActionResult Create()
        {
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName");
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName");
            ViewBag.KeywordID = new SelectList(_context.Keywords, "KeywordID", "Keyword");
            return View();
        }

		// POST: TravelManagement/Hotels/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(HotelsViewModel model)
		{
			if (ModelState.IsValid)
			{
				var hotel = new Hotels
				{
					HotelName = model.HotelName,
					HotelAddr = model.HotelAddr,
					HotelLat = model.HotelLat,
					HotelLng = model.HotelLng,
					HotelDesc = model.HotelDesc,
					RegionID = model.RegionID,
					DistrictID = model.DistrictID,
					Rating = model.Rating,
					Views = model.Views,
					HotelCode = model.HotelCode,
					IsActive = model.IsActive
				};
				_context.Hotels.Add(hotel);
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

							var pic = new HotelPics
							{
								HotelID = hotel.HotelID,
								Picture = ms.ToArray()
							};

							_context.HotelPics.Add(pic);
						}
					}

					await _context.SaveChangesAsync(); // 一次存入所有圖片
				}

				// 關鍵字處理
				if (model.KeywordID != null)
				{
					foreach (var keywordId in model.KeywordID)
					{
						_context.HotelKeywords.Add(new HotelKeywords
						{
							HotelID = hotel.HotelID,
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

		// GET: TravelManagement/Hotels/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			//var hotels = await _context.Hotels.FindAsync(id);
			// 取出 Hotel 及其關聯資料
			var hotels = await _context.Hotels
				.Include(h => h.HotelPics)                // 包含圖片
				.Include(h => h.HotelKeywords)            // 包含關鍵字
				.ThenInclude(hk => hk.Keyword)        // 包含關鍵字詳細資料
				.FirstOrDefaultAsync(h => h.HotelID == id);

			if (hotels == null)
            {
                return NotFound();
            }

			// ✅ 關鍵字：一次投影，保證數量相同
			var keywordList = hotels.HotelKeywords
				.Where(k => k.Keyword != null && k.KeywordID != null)
				.Select(k => new { Id = k.KeywordID.Value, Name = k.Keyword.Keyword })
				.ToList();

			// 塞進 ViewModel
			var viewModel = new HotelsViewModel
			{
				HotelID = hotels.HotelID,
				HotelName = hotels.HotelName,
				HotelAddr = hotels.HotelAddr,
				HotelLat = hotels.HotelLat,
				HotelLng = hotels.HotelLng,
				HotelDesc = hotels.HotelDesc,
				RegionID = hotels.RegionID,
				DistrictID = hotels.DistrictID,
				Rating = hotels.Rating,
				Views = hotels.Views,
				HotelCode = hotels.HotelCode,
				IsActive = hotels.IsActive,

				// 關鍵字
				KeywordID = hotels.HotelKeywords.Select(k => k.KeywordID ?? 0).ToList(),
				KeywordNames = hotels.HotelKeywords.Select(k => k.Keyword.Keyword).ToList(),

				// 圖片
				PictureIds = hotels.HotelPics.Select(p => p.HotelPicID).ToList(),
				PictureBase64 = hotels.HotelPics
					   .Select(p => "data:image/png;base64," + Convert.ToBase64String(p.Picture))
					   .ToList()
			};

			// 下拉選單 / 多選清單
			ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName", hotels.DistrictID);
			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", hotels.RegionID);
			ViewBag.Keywords = new MultiSelectList(_context.Keywords, "KeywordID", "Keyword", viewModel.KeywordID);
						
			return View(viewModel);
		}

        // POST: TravelManagement/Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HotelsViewModel model)
        {
            if (id != model.HotelID)
            {
                return NotFound();
            }

			if (ModelState.IsValid)
            {
				var hotel = await _context.Hotels
					.Include(h => h.HotelPics)
					.Include(h => h.HotelKeywords)
					.FirstOrDefaultAsync(h => h.HotelID == id);

				if (hotel == null) return NotFound();

				// ---- 更新主表資料 ----
				hotel.HotelName = model.HotelName;
				hotel.HotelAddr = model.HotelAddr;
				hotel.HotelLat = model.HotelLat;
				hotel.HotelLng = model.HotelLng;
				hotel.HotelDesc = model.HotelDesc;
				hotel.RegionID = model.RegionID;
				hotel.DistrictID = model.DistrictID;
				hotel.Rating = model.Rating;
				hotel.Views = model.Views;
				hotel.HotelCode = model.HotelCode;
				hotel.IsActive = model.IsActive;

				//// ---- 刪除舊圖片（等按確認才真正刪 DB）----
				if (model.DeletedPictureIds != null && model.DeletedPictureIds.Any())
				{
					var picsToDelete = _context.HotelPics
						.Where(p => model.DeletedPictureIds.Contains(p.HotelPicID))
						.ToList();

					_context.HotelPics.RemoveRange(picsToDelete);
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

							var pic = new HotelPics
							{
								HotelID = hotel.HotelID,
								Picture = ms.ToArray()
							};
							_context.HotelPics.Add(pic);
						}
					}
					await _context.SaveChangesAsync();// 一次存入所有圖片
				}				

				// ---- 更新關鍵字 ---- (先清空再加新的)
				hotel.HotelKeywords.Clear();
				if (model.KeywordID != null && model.KeywordID.Any())
				{
					foreach (var keywordId in model.KeywordID)
					{
						hotel.HotelKeywords.Add(new HotelKeywords
						{
							HotelID = hotel.HotelID,
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
			var pic = await _context.HotelPics.FindAsync(id);
			if (pic == null) return NotFound();

			_context.HotelPics.Remove(pic);
			await _context.SaveChangesAsync();

			return Ok(); // 讓前端 JS 判斷成功
		}

		// GET: TravelManagement/Hotels/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotels = await _context.Hotels
                .Include(h => h.District)
                .Include(h => h.Region)
                .FirstOrDefaultAsync(m => m.HotelID == id);
            if (hotels == null)
            {
                return NotFound();
            }

            return View(hotels);
        }

        // POST: TravelManagement/Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotels = await _context.Hotels.FindAsync(id);
            if (hotels != null)
            {
                _context.Hotels.Remove(hotels);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelsExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelID == id);
        }
    }
}
