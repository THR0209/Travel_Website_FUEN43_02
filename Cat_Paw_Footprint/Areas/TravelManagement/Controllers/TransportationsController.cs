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
				.Include(h => h.TransportPics)
                .Include(h => h.TransportKeywords)
                .ThenInclude(hk => hk.Keyword);

			var viewModel = await transportations.Select(h => new TransportationsViewMoled
			{
                TransportID = h.TransportID,
                TransportName = h.TransportName,
                TransportDesc = h.TransportDesc,
                TransportPrice = h.TransportPrice,
                Rating = h.Rating,
                Views = h.Views,
                TransportCode = h.TransportCode,
                IsActive = h.IsActive,
                Picture = h.TransportPics.Select(p => p.Picture).ToList(),
                Keywords = h.TransportKeywords.
                    Select(k => k.Keyword.KeywordID).ToList()
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
                .FirstOrDefaultAsync(m => m.TransportID == id);

            if (transportations == null)
            {
                return NotFound();
            }

            return View(transportations);
        }

        // GET: TravelManagement/Transportations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TravelManagement/Transportations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransportID,TransportName,TransportDesc,TransportPrice,Rating,Views,TransportCode,IsActive")] Transportations transportations)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transportations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transportations);
        }

        // GET: TravelManagement/Transportations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportations = await _context.Transportations.FindAsync(id);
            if (transportations == null)
            {
                return NotFound();
            }
            return View(transportations);
        }

        // POST: TravelManagement/Transportations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransportID,TransportName,TransportDesc,TransportPrice,Rating,Views,TransportCode,IsActive")] Transportations transportations)
        {
            if (id != transportations.TransportID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transportations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransportationsExists(transportations.TransportID))
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
            return View(transportations);
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
