using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.TravelManagement.Controllers
{
    [Area("TravelManagement")]
    public class HotelsController : Controller
    {
        private readonly webtravel2Context _context;

        public HotelsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: TravelManagement/Hotels
        public async Task<IActionResult> Index()
        {
            var webtravel2Context = _context.Hotels
                .Include(h => h.District)
                .Include(h => h.Region);

            return View(await webtravel2Context.ToListAsync());
        }

        // GET: TravelManagement/Hotels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotels = await _context.Hotels
                .Include(h => h.District)
                .Include(h => h.Region)
                .FirstOrDefaultAsync(m => m.HotelID == id);
            if (hotels == null)
            {
                return NotFound();
            }

            return View(hotels);
        }

        // GET: TravelManagement/Hotels/Create
        public IActionResult Create()
        {
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID");
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID");
            return View();
        }

        // POST: TravelManagement/Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelID,HotelName,HotelAddr,HotelLat,HotelLng,HotelDesc,RegionID,DistrictID,Rating,Views,HotelCode,IsActive")] Hotels hotels)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotels);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", hotels.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", hotels.RegionID);
            return View(hotels);
        }

        // GET: TravelManagement/Hotels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotels = await _context.Hotels.FindAsync(id);
            if (hotels == null)
            {
                return NotFound();
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", hotels.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", hotels.RegionID);
            return View(hotels);
        }

        // POST: TravelManagement/Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HotelID,HotelName,HotelAddr,HotelLat,HotelLng,HotelDesc,RegionID,DistrictID,Rating,Views,HotelCode,IsActive")] Hotels hotels)
        {
            if (id != hotels.HotelID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotels);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelsExists(hotels.HotelID))
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
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", hotels.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", hotels.RegionID);
            return View(hotels);
        }

        // GET: TravelManagement/Hotels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotels = await _context.Hotels
                .Include(h => h.District)
                .Include(h => h.Region)
                .FirstOrDefaultAsync(m => m.HotelID == id);
            if (hotels == null)
            {
                return NotFound();
            }

            return View(hotels);
        }

        // POST: TravelManagement/Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotels = await _context.Hotels.FindAsync(id);
            if (hotels != null)
            {
                _context.Hotels.Remove(hotels);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelsExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelID == id);
        }
    }
}
