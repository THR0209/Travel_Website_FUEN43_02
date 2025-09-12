using Cat_Paw_Footprint.Areas.Admin.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

		// Index
		public async Task<IActionResult> Index()
		{
			var vmList = await _context.Promotions
				.Include(p => p.Products_Promotions)
				.ThenInclude(pp => pp.Product)
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

		// Details
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var p = await _context.Promotions
				.Include(x => x.Products_Promotions)
				.ThenInclude(pp => pp.Product)
				.FirstOrDefaultAsync(x => x.PromoID == id);

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
				Products = p.Products_Promotions.Select(pp => new ProductViewModel
				{
					ProductID = pp.ProductID,
					ProductName = pp.Product.ProductName,
					ProductPrice = pp.Product.ProductPrice
				}).ToList()
			};

			return View(vm);
		}

		// Create (GET)
		public IActionResult Create()
		{
			var vm = new PromotionViewModel
			{
				Products = _context.Products.Select(x => new ProductViewModel
				{
					ProductID = x.ProductID,
					ProductName = x.ProductName,
					ProductPrice = x.ProductPrice
				}).ToList()
			};
			return View(vm);
		}

		// Create (POST)
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

				// 先存 Promotion，確保有正確的 PromoID
				_context.Promotions.Add(promo);
				await _context.SaveChangesAsync();

				// 再存 Products_Promotions
				if (vm.SelectedProductIDs != null && vm.SelectedProductIDs.Any())
				{
					foreach (var productId in vm.SelectedProductIDs)
					{
						_context.Products_Promotions.Add(new Products_Promotions
						{
							PromoID = promo.PromoID,   // 此時已經有值
							ProductID = productId
						});
					}
					await _context.SaveChangesAsync();
				}

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

		// Edit (GET)
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
				Products = _context.Products.Select(x => new ProductViewModel
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

		// Edit (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(PromotionViewModel vm)
		{
			if (ModelState.IsValid)
			{
				// 先找到 Promotion
				var promo = await _context.Promotions.FindAsync(vm.PromoID);
				if (promo == null) return NotFound();

				// 更新基本欄位
				promo.PromoName = vm.PromoName;
				promo.PromoDesc = vm.PromoDesc;
				promo.StartTime = vm.StartTime;
				promo.EndTime = vm.EndTime;
				promo.DiscountType = vm.DiscountType;
				promo.DiscountValue = vm.DiscountValue;
				promo.IsActive = vm.IsActive;
				promo.UpdateTime = DateTime.Now;

				// 存一次，確保 PromoID 還存在資料庫
				await _context.SaveChangesAsync();

				// 先清掉舊的綁定
				var oldLinks = _context.Products_Promotions.Where(pp => pp.PromoID == promo.PromoID);
				_context.Products_Promotions.RemoveRange(oldLinks);
				await _context.SaveChangesAsync();

				// 再新增新的綁定
				if (vm.SelectedProductIDs != null && vm.SelectedProductIDs.Any())
				{
					foreach (var productId in vm.SelectedProductIDs)
					{
						_context.Products_Promotions.Add(new Products_Promotions
						{
							PromoID = promo.PromoID,
							ProductID = productId
						});
					}
					await _context.SaveChangesAsync();
				}

				return RedirectToAction(nameof(Index));
			}

			// 如果驗證失敗，重建產品清單再回傳
			vm.Products = _context.Products
				.Select(x => new ProductViewModel
				{
					ProductID = x.ProductID,
					ProductName = x.ProductName,
					ProductPrice = x.ProductPrice
				}).ToList();

			return View(vm);
		}

		// Delete (GET)
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var promo = await _context.Promotions
				.Include(p => p.Products_Promotions)
					.ThenInclude(pp => pp.Product)
				.FirstOrDefaultAsync(p => p.PromoID == id);

			if (promo == null) return NotFound();

			var vm = new PromotionViewModel
			{
				PromoID = promo.PromoID,
				PromoName = promo.PromoName,
				PromoDesc = promo.PromoDesc,
				StartTime = promo.StartTime,
				EndTime = promo.EndTime,
				DiscountType = promo.DiscountType,
				DiscountValue = promo.DiscountValue,
				IsActive = promo.IsActive,
				CreateTime = promo.CreateTime,
				UpdateTime = promo.UpdateTime,

				Products = promo.Products_Promotions.Select(pp => new ProductViewModel
				{
					ProductID = pp.ProductID,
					ProductName = pp.Product.ProductName,
					ProductPrice = pp.Product.ProductPrice
				}).ToList()
			};

			return View(vm);
		}

		// Delete (POST)
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			// 先找 Promotion
			var promo = await _context.Promotions.FindAsync(id);
			if (promo != null)
			{
				// 🔹 先刪掉 Products_Promotions 的關聯
				var links = _context.Products_Promotions.Where(pp => pp.PromoID == id);
				_context.Products_Promotions.RemoveRange(links);
				await _context.SaveChangesAsync();

				// 🔹 再刪 Promotion
				_context.Promotions.Remove(promo);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
