using Cat_Paw_Footprint.Areas.CouponManagement.ViewModel;
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

namespace Cat_Paw_Footprint.Areas.CouponManagement.Controllers
{
	[Area("CouponManagement")]
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaCouponManagement")]
	public class CouponsController : Controller
	{
		private readonly webtravel2Context _context;

		public CouponsController(webtravel2Context context)
		{
			_context = context;
		}

		// GET: CouponManagement/Coupons
		public async Task<IActionResult> Index()
		{
			var coupons = await _context.Coupons.Select(c => new CouponViewModel
			{
				CouponID = c.CouponID,
				CouponName = c.CouponName,
				DiscountType = c.DiscountType,
				DiscountValue = c.DiscountValue,
				StartDate = c.StartDate,
				EndDate = c.EndDate,
				IsActive = c.IsActive
			}).ToListAsync();

			return View(coupons);

		}

		// GET: CouponManagement/Coupons/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
				return NotFound();

			var coupon = await _context.Coupons
				.Where(c => c.CouponID == id)
				.Select(c => new CouponViewModel
				{
					CouponID = c.CouponID,
					CouponName = c.CouponName,
					CouponDesc = c.CouponDesc,
					DiscountType = c.DiscountType,
					DiscountValue = c.DiscountValue,
					StartDate = c.StartDate,
					EndDate = c.EndDate,
					IsActive = c.IsActive,
					TargetType = c.TargetType,
					DiscountCode = c.DiscountCode
				})
				.FirstOrDefaultAsync();

			if (coupon == null)
				return NotFound();

			return View(coupon);
		}


		// GET: CouponManagement/Coupons/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: CouponManagement/Coupons/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CouponViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			var coupon = new Coupons
			{
				CouponName = vm.CouponName,
				CouponDesc = vm.CouponDesc,
				DiscountType = vm.DiscountType,
				DiscountValue = vm.DiscountValue,
				StartDate = vm.StartDate,
				EndDate = vm.EndDate,
				IsActive = vm.IsActive,
				DiscountCode = vm.DiscountCode,
				TargetType = vm.TargetType,
				CreatedAt = DateTime.Now,
				CreatedBy = User.Identity?.Name ?? "System"
			};

			_context.Coupons.Add(coupon);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
			
		}

        // GET: CouponManagement/Coupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();

            var vm = new CouponViewModel
            {
                CouponID = coupon.CouponID,
                CouponName = coupon.CouponName,
                CouponDesc = coupon.CouponDesc,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                IsActive = coupon.IsActive,
                TargetType = coupon.TargetType,
                DiscountCode = coupon.DiscountCode,
                MinimumAmount = coupon.MinimumAmount
            };

            return View(vm);
        }


        // POST: CouponManagement/Coupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CouponViewModel vm)
        {
            if (id != vm.CouponID)
                return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
                return NotFound();

            // 手動映射回實體
            coupon.CouponName = vm.CouponName;
            coupon.CouponDesc = vm.CouponDesc;
            coupon.DiscountType = vm.DiscountType;
            coupon.DiscountValue = vm.DiscountValue;
            coupon.StartDate = vm.StartDate;
            coupon.EndDate = vm.EndDate;
            coupon.IsActive = vm.IsActive;
            coupon.TargetType = vm.TargetType;
            coupon.DiscountCode = vm.DiscountCode;
            coupon.MinimumAmount = vm.MinimumAmount;
            coupon.UpdatedAt = DateTime.Now;
            coupon.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: CouponManagement/Coupons/Delete/5
        public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var coupons = await _context.Coupons
				.FirstOrDefaultAsync(m => m.CouponID == id);
			if (coupons == null)
			{
				return NotFound();
			}

			return View(coupons);
		}

		// POST: CouponManagement/Coupons/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var coupons = await _context.Coupons.FindAsync(id);
			if (coupons != null)
			{
				_context.Coupons.Remove(coupons);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool CouponsExists(int id)
		{
			return _context.Coupons.Any(e => e.CouponID == id);
		}
	}
}
