using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

// 若你有與 Products 相同的 ImgBB 上傳 Helper，請取消下一行註解並調整命名空間
using Cat_Paw_Footprint.Areas.Helper;  // ImgBBHelper.UploadSingleImageAsync(IFormFile file)

namespace Cat_Paw_Footprint.Areas.ProductManagement.Controllers
{
	[Area("ProductManagement")]
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaProductManagement")]
	public class SemiSelfProductsController : Controller
	{
		private readonly webtravel2Context _context;

		public SemiSelfProductsController(webtravel2Context context)
		{
			_context = context;
		}

		// =========================
		// Index & DataTables Json
		// =========================
		public async Task<IActionResult> Index()
		{
			var list = await _context.SemiSelfProducts
									 .Include(s => s.Region)
									 .AsNoTracking()
									 .ToListAsync();
			return View(list);
		}

		// 提供 DataTables 的資料來源（與 Products 一致的風格）
		[HttpPost]
		[Route("ProductManagement/SemiSelfProducts/Index/Json")]
		public async Task<IActionResult> IndexJson()
		{
			var data = await _context.SemiSelfProducts
				.Include(s => s.Region)
				.AsNoTracking()
				.OrderByDescending(s => s.ProductCode)
				.Select(s => new
				{
					s.ProductID,
					s.ProductCode,
					s.ProductImageUrl,
					s.ProductName,
					s.ProductPrice,
					StartDate = s.StartDate,
					// SemiSelf 沒有 ProductAnalysis 的上架時間；這裡以 CreateTime 當「上架時間」展示
					CreateTime = s.CreateTime,
					s.MaxPeople,
					s.RegionID,
					s.IsActive,
					RegionName = s.Region == null ? null : s.Region.RegionName
				})
				.ToListAsync();

			return Json(data);
		}

		// =========================
		// Details
		// =========================
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var model = await _context.SemiSelfProducts
				.Include(s => s.Region)
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.ProductID == id);
			if (model == null) return NotFound();

			return View(model);
		}

		// GET: ProductManagement/SemiSelfProducts/Create
		public IActionResult Create()
		{
			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName");
			ViewData["KeywordID"] = new SelectList(_context.Keywords.OrderBy(k => k.KeywordID), "KeywordID", "Keyword");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SemiSelfProducts model, IFormFile? UploadImage, List<int>? KeywordID)
		{
			if (!ModelState.IsValid)
			{
				ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", model.RegionID);
				ViewData["KeywordID"] = new SelectList(_context.Keywords, "KeywordID", "Keyword");
				return View(model);
			}

			if (UploadImage != null && UploadImage.Length > 0)
				model.ProductImageUrl = await ImgBBHelper.UploadSingleImageAsync(UploadImage);

			model.ProductCode = await GenerateProductCodeAsync();
			model.CreateTime = DateTime.Now;
			model.UpdateTime = DateTime.Now;

			_context.Add(model);
			await _context.SaveChangesAsync();

			// 🔹關鍵字關聯寫入 Semi_Keywords
			if (KeywordID != null && KeywordID.Any())
			{
				foreach (var kid in KeywordID)
				{
					_context.Semi_Keywords.Add(new Semi_Keywords
					{
						ProductID = model.ProductID,
						KeywordID = kid
					});
				}
				await _context.SaveChangesAsync();
			}

			TempData["SuccessMessage"] = "新增成功！";
			return RedirectToAction(nameof(Index));
		}


		// GET: ProductManagement/SemiSelfProducts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var entity = await _context.SemiSelfProducts
				//.Include(p => p.SemiKeywords)
				//.ThenInclude(sk => sk.Keyword)
				.FirstOrDefaultAsync(p => p.ProductID == id);
			if (entity == null) return NotFound();

			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", entity.RegionID);
			//ViewData["KeywordID"] = new SelectList(_context.Keywords, "KeywordID", "Keyword");

			//ViewBag.SelectedKeywordIDs = entity.SemiKeywords.Select(sk => sk.KeywordID).ToList();
			//ViewBag.SelectedKeywordNames = entity.SemiKeywords.Select(sk => sk.Keyword.Keyword).ToList();

			return View(entity);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, SemiSelfProducts model, IFormFile? UploadImage)
		{
			if (id != model.ProductID) return NotFound();

			var entity = await _context.SemiSelfProducts
				//.Include(p => p.SemiKeywords)
				.FirstOrDefaultAsync(p => p.ProductID == id);
			if (entity == null) return NotFound();

			if (UploadImage != null && UploadImage.Length > 0)
				entity.ProductImageUrl = await ImgBBHelper.UploadSingleImageAsync(UploadImage);

			entity.ProductName = model.ProductName;
			entity.RegionID = model.RegionID;
			entity.ProductDesc = model.ProductDesc;
			entity.ProductPrice = model.ProductPrice;
			entity.StartDate = model.StartDate;
			entity.EndTime = model.EndTime;
			entity.IsActive = model.IsActive;
			entity.MaxPeople = model.MaxPeople;
			entity.Notes = model.Notes;
			entity.UpdateTime = DateTime.Now;

			// 🔹重建關鍵字關聯
			//_context.Semi_Keywords.RemoveRange(entity.SemiKeywords);
			//if (KeywordID != null && KeywordID.Any())
			//{
			//	foreach (var kid in KeywordID)
			//	{
			//		_context.Semi_Keywords.Add(new Semi_Keywords
			//		{
			//			ProductID = entity.ProductID,
			//			KeywordID = kid
			//		});
			//	}
			//}

			await _context.SaveChangesAsync();
			TempData["SuccessMessage"] = "修改成功！";
			return RedirectToAction(nameof(Index));
		}

		// =========================
		// Delete
		// =========================
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var model = await _context.SemiSelfProducts
				.Include(s => s.Region)
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.ProductID == id);
			if (model == null) return NotFound();

			return View(model);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var model = await _context.SemiSelfProducts.FindAsync(id);
			if (model != null) _context.SemiSelfProducts.Remove(model);

			await _context.SaveChangesAsync();
			TempData["SuccessMessage"] = "已刪除！";
			return RedirectToAction(nameof(Index));
		}

		// =========================
		// 軟下架（同 Products 風格）
		// =========================
		[HttpPost]
		[Route("ProductManagement/SemiSelfProducts/Deactivate")]
		public async Task<IActionResult> Deactivate(int id)
		{
			var item = await _context.SemiSelfProducts.FindAsync(id);
			if (item == null) return NotFound();

			item.IsActive = false;
			item.UpdateTime = DateTime.Now;
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		private bool SemiSelfProductsExists(int id)
		{
			return _context.SemiSelfProducts.Any(e => e.ProductID == id);
		}

		private async Task<string> GenerateProductCodeAsync()
		{
			var now = await GetDatabaseNowAsync(); // ← 用資料庫時間
			var today = now.Date;

			// 計算今天已有幾筆
			var dailyCount = await _context.Products
				.CountAsync(p => EF.Functions.DateDiffDay(p.CreateTime, today) == 0);

			var serial = (dailyCount + 1).ToString("D4"); // 四位數補 0
			return $"SSP{now:yyMMdd}{serial}";
		}

		private async Task<DateTime> GetDatabaseNowAsync()
		{
			// 直接從 DB 取 GETDATE()
			var conn = _context.Database.GetDbConnection();
			await conn.OpenAsync();
			using var cmd = conn.CreateCommand();
			cmd.CommandText = "SELECT GETDATE()";
			var result = await cmd.ExecuteScalarAsync();
			return Convert.ToDateTime(result);
		}
	}
}
