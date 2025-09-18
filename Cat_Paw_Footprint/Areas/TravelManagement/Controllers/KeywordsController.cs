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
	public class KeywordsController : Controller
    {
        private readonly webtravel2Context _context;

        public KeywordsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: TravelManagement/Keywords
        public async Task<IActionResult> Index()
        {
            return View(await _context.Keywords.ToListAsync());
        }

        // GET: TravelManagement/Keywords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keywords = await _context.Keywords
                .FirstOrDefaultAsync(m => m.KeywordID == id);
            if (keywords == null)
            {
                return NotFound();
            }

            return View(keywords);
        }

        // GET: TravelManagement/Keywords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TravelManagement/Keywords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KeywordID,Keyword,Views")] Keywords keywords)
        {
            if (ModelState.IsValid)
            {
                _context.Add(keywords);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(keywords);
        }

        // GET: TravelManagement/Keywords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keywords = await _context.Keywords.FindAsync(id);
            if (keywords == null)
            {
                return NotFound();
            }
            return View(keywords);
        }

        // POST: TravelManagement/Keywords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KeywordID,Keyword,Views")] Keywords keywords)
        {
            if (id != keywords.KeywordID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(keywords);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeywordsExists(keywords.KeywordID))
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
            return View(keywords);
        }

        // GET: TravelManagement/Keywords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keywords = await _context.Keywords
                .FirstOrDefaultAsync(m => m.KeywordID == id);
            if (keywords == null)
            {
                return NotFound();
            }

            return View(keywords);
        }

        // POST: TravelManagement/Keywords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var keywords = await _context.Keywords.FindAsync(id);
            if (keywords != null)
            {
                _context.Keywords.Remove(keywords);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KeywordsExists(int id)
        {
            return _context.Keywords.Any(e => e.KeywordID == id);
        }
    }
}
