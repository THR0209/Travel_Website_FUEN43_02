using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Data;

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

        // GET: Admin/Promotions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Promotions.ToListAsync());
        }

        // GET: Admin/Promotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotions = await _context.Promotions
                .FirstOrDefaultAsync(m => m.PromoID == id);
            if (promotions == null)
            {
                return NotFound();
            }

            return View(promotions);
        }

        // GET: Admin/Promotions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Promotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PromoID,PromoName,PromoDesc,StartTime,EndTime,DiscountType,DiscountValue,IsActive,CreateTime,UpdateTime")] Promotions promotions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promotions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(promotions);
        }

        // GET: Admin/Promotions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotions = await _context.Promotions.FindAsync(id);
            if (promotions == null)
            {
                return NotFound();
            }
            return View(promotions);
        }

        // POST: Admin/Promotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PromoID,PromoName,PromoDesc,StartTime,EndTime,DiscountType,DiscountValue,IsActive,CreateTime,UpdateTime")] Promotions promotions)
        {
            if (id != promotions.PromoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promotions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionsExists(promotions.PromoID))
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
            return View(promotions);
        }

        // GET: Admin/Promotions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotions = await _context.Promotions
                .FirstOrDefaultAsync(m => m.PromoID == id);
            if (promotions == null)
            {
                return NotFound();
            }

            return View(promotions);
        }

        // POST: Admin/Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promotions = await _context.Promotions.FindAsync(id);
            if (promotions != null)
            {
                _context.Promotions.Remove(promotions);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PromotionsExists(int id)
        {
            return _context.Promotions.Any(e => e.PromoID == id);
        }
    }
}
