using Cat_Paw_Footprint.Areas.Admin.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class PromotionsController : Controller
	{
		private readonly webtravel2Context _context;

		public PromotionsController(webtravel2Context context)
		{
			_context = context;
		}

		// ✅ Index: 活動列表 + 綁定產品
		public async Task<IActionResult> Index()
		{
			var vmList = await _context.Promotions
	   .Include(p => p.Products_Promotions)      // 先抓中介表
		   .ThenInclude(pp => pp.Product)       // 再抓產品
	   .Select(p => new PromotionViewModel
	   {
		   PromoID = p.PromoID,
		   PromoName = p.PromoName,
		   PromoDesc = p.PromoDesc,
		   StartTime = p.StartTime,
		   EndTime = p.EndTime,
		   DiscountType = p.DiscountType,
		   DiscountValue = p.DiscountValue,
		   IsActive = p.IsActive,
		   CreateTime = p.CreateTime,
		   UpdateTime = p.UpdateTime,

		   Products = p.Products_Promotions.Select(pp => new ProductViewModel
		   {
			   ProductID = pp.ProductID,
			   ProductName = pp.Product.ProductName,
			   ProductPrice = pp.Product.ProductPrice
		   }).ToList()
	   })
	   .ToListAsync();

			return View(vmList);
		}

		// ✅ Details: 活動詳細
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var p = await _context.Promotions.FirstOrDefaultAsync(x => x.PromoID == id);
			if (p == null) return NotFound();

			var vm = new PromotionViewModel
			{
				PromoID = p.PromoID,
				PromoName = p.PromoName,
				PromoDesc = p.PromoDesc,
				StartTime = p.StartTime,
				EndTime = p.EndTime,
				DiscountType = p.DiscountType,
				DiscountValue = p.DiscountValue,
				IsActive = p.IsActive,
				CreateTime = p.CreateTime,
				UpdateTime = p.UpdateTime,
				Products = _context.Products_Promotions
					.Where(pp => pp.PromoID == p.PromoID)
					.Select(pp => new ProductViewModel
					{
						ProductID = pp.ProductID,
						ProductName = pp.Product.ProductName,
						ProductPrice = pp.Product.ProductPrice
					}).ToList()
			};

			return View(vm);
		}

		// ✅ Create (GET)
		public IActionResult Create()
		{
			var vm = new PromotionViewModel
			{
				Products = _context.Products
					.Select(x => new ProductViewModel
					{
						ProductID = x.ProductID,
						ProductName = x.ProductName,
						ProductPrice = x.ProductPrice
					}).ToList()
			};
			return View(vm);
		}

		// ✅ Create (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(PromotionViewModel vm)
		{
			if (ModelState.IsValid)
			{
				var promo = new Promotions
				{
					PromoName = vm.PromoName,
					PromoDesc = vm.PromoDesc,
					StartTime = vm.StartTime,
					EndTime = vm.EndTime,
					DiscountType = vm.DiscountType,
					DiscountValue = vm.DiscountValue,
					IsActive = vm.IsActive,
					CreateTime = DateTime.Now,
					UpdateTime = DateTime.Now
				};

				_context.Promotions.Add(promo);
				await _context.SaveChangesAsync();

				foreach (var productId in vm.SelectedProductIDs)
				{
					_context.Products_Promotions.Add(new Products_Promotions
					{
						PromoID = promo.PromoID,
						ProductID = productId
					});
				}
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}

			// 如果失敗，重建產品清單再回傳
			vm.Products = _context.Products
				.Select(x => new ProductViewModel
				{
					ProductID = x.ProductID,
					ProductName = x.ProductName,
					ProductPrice = x.ProductPrice
				}).ToList();

			return View(vm);
		}

		// ✅ Edit (GET)
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var p = await _context.Promotions.FindAsync(id);
			if (p == null) return NotFound();

			var vm = new PromotionViewModel
			{
				PromoID = p.PromoID,
				PromoName = p.PromoName,
				PromoDesc = p.PromoDesc,
				StartTime = p.StartTime,
				EndTime = p.EndTime,
				DiscountType = p.DiscountType,
				DiscountValue = p.DiscountValue,
				IsActive = p.IsActive,
				CreateTime = p.CreateTime,
				UpdateTime = p.UpdateTime,
				Products = _context.Products
					.Select(x => new ProductViewModel
					{
						ProductID = x.ProductID,
						ProductName = x.ProductName,
						ProductPrice = x.ProductPrice
					}).ToList(),
				SelectedProductIDs = _context.Products_Promotions
					.Where(pp => pp.PromoID == p.PromoID)
					.Select(pp => pp.ProductID)
					.ToList()
			};

			return View(vm);
		}

		// ✅ Edit (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(PromotionViewModel vm)
		{
			if (ModelState.IsValid)
			{
				var promo = await _context.Promotions.FindAsync(vm.PromoID);
				if (promo == null) return NotFound();

				promo.PromoName = vm.PromoName;
				promo.PromoDesc = vm.PromoDesc;
				promo.StartTime = vm.StartTime;
				promo.EndTime = vm.EndTime;
				promo.DiscountType = vm.DiscountType;
				promo.DiscountValue = vm.DiscountValue;
				promo.IsActive = vm.IsActive;
				promo.UpdateTime = DateTime.Now;

				// 清掉舊的產品綁定
				var oldLinks = _context.Products_Promotions.Where(pp => pp.PromoID == vm.PromoID);
				_context.Products_Promotions.RemoveRange(oldLinks);

				// 新增新的綁定
				foreach (var productId in vm.SelectedProductIDs)
				{
					_context.Products_Promotions.Add(new Products_Promotions
					{
						PromoID = promo.PromoID,
						ProductID = productId
					});
				}

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			// 如果失敗，重建產品清單再回傳
			vm.Products = _context.Products
				.Select(x => new ProductViewModel
				{
					ProductID = x.ProductID,
					ProductName = x.ProductName,
					ProductPrice = x.ProductPrice
				}).ToList();

			return View(vm);
		}

		// ✅ Delete (GET)
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var promo = await _context.Promotions.FindAsync(id);
			if (promo == null) return NotFound();

			var vm = new PromotionViewModel
			{
				PromoID = promo.PromoID,
				PromoName = promo.PromoName,
				PromoDesc = promo.PromoDesc
			};

			return View(vm);
		}

		// ✅ Delete (POST)
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var promo = await _context.Promotions.FindAsync(id);
			if (promo != null)
			{
				_context.Promotions.Remove(promo);

				var links = _context.Products_Promotions.Where(pp => pp.PromoID == id);
				_context.Products_Promotions.RemoveRange(links);

				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Index));
		}
	}
}
