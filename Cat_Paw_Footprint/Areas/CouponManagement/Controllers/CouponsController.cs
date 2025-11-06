using Cat_Paw_Footprint.Areas.CouponManagement.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Services;
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
        private readonly INotificationTriggerService _notifTrigger;

        public CouponsController(webtravel2Context context, INotificationTriggerService notifTrigger)
		{
			_context = context;
            _notifTrigger = notifTrigger;
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
			{
                return View(vm);
            }
                

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
            //coupon.MinimumAmount = vm.MinimumAmount;
            //coupon.UpdatedAt = DateTime.Now;
            coupon.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: CouponManagement/Coupons/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
                    DiscountCode = c.DiscountCode
                })
                .FirstOrDefaultAsync();

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        // POST: CouponManagement/Coupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
                return NotFound();

            // 改為下架（IsActive = false）
            coupon.IsActive = false;
            coupon.UpdatedAt = DateTime.Now;
            coupon.UpdatedBy = User.Identity?.Name ?? "System";

            _context.Update(coupon);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Distribute(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                TempData["Error"] = "找不到該優惠券。";
                return RedirectToAction(nameof(Index));
            }

            var now = DateTime.Now;
            var targets = _context.Customers.AsQueryable();

            // 🎯 根據 TargetType 判斷發放對象
            switch (coupon.TargetType)
            {
                case "All":
                    targets = targets.Where(c => c.Status == true);
                    break;
                case "Register":
                    // 可改成註冊3天內會員
                    targets = targets.Where(c => c.CreateDate >= now.AddDays(-3));
                    break;
                case "Level_Bronze":
                    targets = targets.Where(c => c.Level == 1);
                    break;
                case "Level_Silver":
                    targets = targets.Where(c => c.Level == 2);
                    break;
                case "Level_Gold":
                    targets = targets.Where(c => c.Level == 3);
                    break;
                default:
                    TempData["Error"] = "未定義的發放對象類型。";
                    return RedirectToAction(nameof(Index));
            }

            var targetList = await targets.ToListAsync();
            int count = 0;

            foreach (var member in targetList)
            {
                bool alreadyHas = await _context.CustomerCouponsRecords
                    .AnyAsync(r => r.CustomerID == member.CustomerID && r.CouponID == coupon.CouponID);

                if (!alreadyHas)
                {
                    DateTime usedTime = DateTime.Now;

                    // 🔹 若優惠券有設定 ValidDays，就用「領取時間 + ValidDays」為 ExpireTime
                    // 🔹 否則退回用主表的 EndDate
                    DateTime? expireTime = null;

                    // ✅ 有 ValidDays 則以領取時間 + ValidDays 為到期日
                    if (coupon.ValidDays.HasValue && coupon.ValidDays.Value > 0)
                    {
                        expireTime = usedTime.AddDays(coupon.ValidDays.Value);
                    }
                    // ✅ 沒有 ValidDays 就看 EndDate 是否有值
                    else if (coupon.EndDate != default(DateTime))
                    {
                        expireTime = coupon.EndDate;
                    }

                    _context.CustomerCouponsRecords.Add(new CustomerCouponsRecords
                    {
                        CustomerID = member.CustomerID,
                        CouponID = coupon.CouponID,
                        IsUsed = false,
                        UsedTime = usedTime,
                        ExpireTime = expireTime
                    });

                    // ✅ 發送通知（含 SignalR + Email）
                    await _notifTrigger.NotifyCouponIssuedAsync(member.CustomerID, coupon.CouponID);
                    count++;
                }
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = $"已成功發放 {count} 張優惠券給對應會員。";


            return RedirectToAction(nameof(Index));
        }


    }
}
