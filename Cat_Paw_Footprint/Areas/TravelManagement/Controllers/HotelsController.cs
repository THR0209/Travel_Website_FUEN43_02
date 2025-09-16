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
				//Picture = hotel.HotelPics.Select(p => p.Picture).ToList(),
				KeywordID = hotel.HotelKeywords.Select(k => k.Keyword.KeywordID).ToList(),
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

            var hotels = await _context.Hotels.FindAsync(id);
            if (hotels == null)
            {
                return NotFound();
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", hotels.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", hotels.RegionID);
            return View(hotels);
        }

        // POST: TravelManagement/Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HotelID,HotelName,HotelAddr,HotelLat,HotelLng,HotelDesc,RegionID,DistrictID,Rating,Views,HotelCode,IsActive")] Hotels hotels)
        {
            if (id != hotels.HotelID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotels);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelsExists(hotels.HotelID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", hotels.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", hotels.RegionID);
            return View(hotels);
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
