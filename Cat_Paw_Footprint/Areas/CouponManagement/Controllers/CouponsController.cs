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
            return View(await _context.Coupons.ToListAsync());
        }

        // GET: CouponManagement/Coupons/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public async Task<IActionResult> Create([Bind("CouponID,CouponCode,CouponDesc,DiscountType,DiscountValue,StartDate,EndTime,IsActive")] Coupons coupons)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coupons);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }

        // GET: CouponManagement/Coupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupons = await _context.Coupons.FindAsync(id);
            if (coupons == null)
            {
                return NotFound();
            }
            return View(coupons);
        }

        // POST: CouponManagement/Coupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CouponID,CouponCode,CouponDesc,DiscountType,DiscountValue,StartDate,EndTime,IsActive")] Coupons coupons)
        {
            if (id != coupons.CouponID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coupons);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponsExists(coupons.CouponID))
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
            return View(coupons);
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
