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

		// =========================
		// Create
		// =========================
		public IActionResult Create()
		{
			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SemiSelfProducts model, IFormFile? UploadImage)
		{
			if (!ModelState.IsValid)
			{
				ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", model.RegionID);
				return View(model);
			}

			// 封面照上傳（與 Products 同邏輯：以 URL 儲存）
			if (UploadImage != null && UploadImage.Length > 0)
			{
				// 如果專案尚未加入 ImgBBHelper，請改為你的上傳實作或先移除此段
				model.ProductImageUrl = await ImgBBHelper.UploadSingleImageAsync(UploadImage);
			}

			model.CreateTime = DateTime.Now;
			model.UpdateTime = DateTime.Now;

			_context.Add(model);
			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "新增成功！";
			return RedirectToAction(nameof(Index));
		}

		// =========================
		// Edit
		// =========================
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var entity = await _context.SemiSelfProducts.FindAsync(id);
			if (entity == null) return NotFound();

			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", entity.RegionID);
			return View(entity);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, SemiSelfProducts model, IFormFile? UploadImage)
		{
			if (id != model.ProductID) return NotFound();

			if (!ModelState.IsValid)
			{
				ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", model.RegionID);
				return View(model);
			}

			var entity = await _context.SemiSelfProducts.FirstOrDefaultAsync(x => x.ProductID == id);
			if (entity == null) return NotFound();

			// 更新允許編輯的欄位
			entity.ProductName = model.ProductName;
			entity.RegionID = model.RegionID;
			entity.ProductDesc = model.ProductDesc;
			entity.ProductPrice = model.ProductPrice;
			entity.ProductType = model.ProductType;
			entity.StartDate = model.StartDate;
			entity.EndTime = model.EndTime;
			entity.IsActive = model.IsActive;
			entity.Views = model.Views;
			entity.Notes = model.Notes;
			entity.MaxPeople = model.MaxPeople;
			entity.ProductCode = model.ProductCode;  // 若由觸發器產生可忽略
			entity.UpdateTime = DateTime.Now;

			if (UploadImage != null && UploadImage.Length > 0)
			{
				entity.ProductImageUrl = await ImgBBHelper.UploadSingleImageAsync(UploadImage);
			}

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
	}
}
