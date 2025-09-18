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
	public class TransportationsController : Controller
    {
        private readonly webtravel2Context _context;

        public TransportationsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: TravelManagement/Transportations
        public async Task<IActionResult> Index()
        {
            var transportations = _context.Transportations
				.Include(t => t.TransportPics)
                .Include(t => t.TransportKeywords)
                .ThenInclude(hk => hk.Keyword);

			var viewModel = await transportations.Select(t => new TransportationsViewModel
			{
                TransportID = t.TransportID,
                TransportName = t.TransportName,
                TransportDesc = t.TransportDesc,
                TransportPrice = t.TransportPrice,
                Rating = t.Rating,
                Views = t.Views,
                TransportCode = t.TransportCode,
                IsActive = t.IsActive                
            }).ToListAsync();

            return View(viewModel);

		}

        // GET: TravelManagement/Transportations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportations = await _context.Transportations
                .Include(t => t.TransportPics)
                .Include(t => t.TransportKeywords)
                .ThenInclude(hk => hk.Keyword)
				.FirstOrDefaultAsync(t => t.TransportID == id);

            if (transportations == null)
            {
                return NotFound();
            }

            var viewModel = new TransportationsViewModel
            {
                TransportID = transportations.TransportID,
                TransportName = transportations.TransportName,
                TransportDesc = transportations.TransportDesc,
                TransportPrice = transportations.TransportPrice,
                Rating = transportations.Rating,
                Views = transportations.Views,
                TransportCode = transportations.TransportCode,
                IsActive = transportations.IsActive,
				KeywordID = transportations.TransportKeywords.Select(k => k.Keyword.KeywordID).ToList(),
				KeywordNames = transportations.TransportKeywords.Select(k => k.Keyword.Keyword).ToList(),
				PictureBase64 = transportations.TransportPics.Select(p => "data:image/png;base64," + Convert.ToBase64String(p.Picture)).ToList()
			};

			return View(viewModel);
        }

        // GET: TravelManagement/Transportations/Create
        public IActionResult Create()
        {
			ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName");
			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName");
			ViewBag.KeywordID = new SelectList(_context.Keywords, "KeywordID", "Keyword");
			return View();
		}

        // POST: TravelManagement/Transportations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransportationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var transportations = new Transportations
                {
                    TransportName = model.TransportName,
                    TransportDesc = model.TransportDesc,
                    TransportPrice = model.TransportPrice,
                    Rating = model.Rating,
                    Views = model.Views,
                    TransportCode = model.TransportCode,
                    IsActive = model.IsActive
                };
				_context.Transportations.Add(transportations);
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

							var pic = new TransportPics
							{
								TransportID = transportations.TransportID,
								Picture = ms.ToArray()
							};

							_context.TransportPics.Add(pic);
						}
					}

					await _context.SaveChangesAsync(); // 一次存入所有圖片
				}
				// 關鍵字處理
				if (model.KeywordID != null)
				{
					foreach (var keywordId in model.KeywordID)
					{
						_context.TransportKeywords.Add(new TransportKeywords
						{
							TransportID = transportations.TransportID,
							KeywordID = keywordId
						});
					}
				}
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
            }
			// 如果失敗，回傳表單內容
			ViewData["KeywordID"] = new MultiSelectList(_context.Keywords, "KeywordID", "KeywordName", model.KeywordID);
			return View(model);
		}

        // GET: TravelManagement/Transportations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportations = await _context.Transportations
                .Include(t => t.TransportPics)  // 包含圖片
				.Include(t => t.TransportKeywords)  // 包含關鍵字
				.ThenInclude(hk => hk.Keyword)  // 包含關鍵字詳細資料
				.FirstOrDefaultAsync(t => t.TransportID == id);


			if (transportations == null)
            {
                return NotFound();
            }

			// ✅ 關鍵字：一次投影，保證數量相同
			var keywordList = transportations.TransportKeywords
				.Where(k => k.Keyword != null && k.KeywordID != null)
				.Select(k => new { Id = k.KeywordID.Value, Name = k.Keyword.Keyword })
				.ToList();

			// 塞進 ViewModel
            var viewModel = new TransportationsViewModel
            {
                TransportID = transportations.TransportID,
                TransportName = transportations.TransportName,
                TransportDesc = transportations.TransportDesc,
                TransportPrice = transportations.TransportPrice,
                Rating = transportations.Rating,
                Views = transportations.Views,
                TransportCode = transportations.TransportCode,
                IsActive = transportations.IsActive,

				// 關鍵字
				KeywordID = transportations.TransportKeywords.Select(k => k.KeywordID ?? 0).ToList(),
				KeywordNames = transportations.TransportKeywords.Select(k => k.Keyword.Keyword).ToList(),

				// 圖片
				PictureIds = transportations.TransportPics.Select(p => p.TransportPicID).ToList(),
				PictureBase64 = transportations.TransportPics
					   .Select(p => "data:image/png;base64," + Convert.ToBase64String(p.Picture))
					   .ToList()
			};

			// 下拉選單 / 多選清單
			ViewBag.Keywords = new MultiSelectList(_context.Keywords, "KeywordID", "Keyword", viewModel.KeywordID);

			return View(viewModel);
		}

        // POST: TravelManagement/Transportations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,TransportationsViewModel model )
        {
            if (id != model.TransportID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
               var transportations = await _context.Transportations
                    .Include(t => t.TransportPics)
                    .Include(t => t.TransportKeywords)
                    .FirstOrDefaultAsync(t => t.TransportID == id);

                if (transportations == null) return NotFound();

				// ---- 更新主表資料 ----
                transportations.TransportName = model.TransportName;
                transportations.TransportDesc = model.TransportDesc;
                transportations.TransportPrice = model.TransportPrice;
                transportations.Rating = model.Rating;
                transportations.Views = model.Views;
                transportations.TransportCode = model.TransportCode;
                transportations.IsActive = model.IsActive;

				// ---- 刪除舊圖片（等按確認才真正刪 DB）----
				if (model.DeletedPictureIds != null && model.DeletedPictureIds.Any())
				{
					var picsToDelete = _context.TransportPics
						.Where(p => model.DeletedPictureIds.Contains(p.TransportPicID))
						.ToList();

					_context.TransportPics.RemoveRange(picsToDelete);
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

							var pic = new TransportPics
							{
								TransportID = transportations.TransportID,
								Picture = ms.ToArray()
							};
							_context.TransportPics.Add(pic);
						}
					}
					await _context.SaveChangesAsync();// 一次存入所有圖片
				}

				// ---- 更新關鍵字 ---- (先清空再加新的)
				transportations.TransportKeywords.Clear();
				if (model.KeywordID != null && model.KeywordID.Any())
				{
					foreach (var keywordId in model.KeywordID)
					{
						transportations.TransportKeywords.Add(new TransportKeywords
						{
							TransportID = transportations.TransportID,
							KeywordID = keywordId
						});
					}
				}
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			// 驗證失敗回傳畫面
			ViewBag.Keywords = new MultiSelectList(_context.Keywords, "Keyword", "Keyword", model.KeywordID);

			return View(model);
		}

		// ---- 單張刪除圖片 API (AJAX 用) ----
		[HttpPost]
		public async Task<IActionResult> DeletePicture(int id)
		{
			var pic = await _context.TransportPics.FindAsync(id);
			if (pic == null) return NotFound();

			_context.TransportPics.Remove(pic);
			await _context.SaveChangesAsync();

			return Ok(); // 讓前端 JS 判斷成功
		}

		// GET: TravelManagement/Transportations/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportations = await _context.Transportations
                .FirstOrDefaultAsync(m => m.TransportID == id);
            if (transportations == null)
            {
                return NotFound();
            }

            return View(transportations);
        }

        // POST: TravelManagement/Transportations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transportations = await _context.Transportations.FindAsync(id);
            if (transportations != null)
            {
                _context.Transportations.Remove(transportations);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransportationsExists(int id)
        {
            return _context.Transportations.Any(e => e.TransportID == id);
        }
    }
}
