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
    public class LocationsController : Controller
    {
        private readonly webtravel2Context _context;

        public LocationsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: TravelManagement/Locations
        public async Task<IActionResult> Index()
        {
			var location = _context.Locations
				.Include(h => h.LocationPics)
				.Include(h => h.LocationKeywords)
				.ThenInclude(hk => hk.Keyword)
				.Include(h => h.District)
				.Include(h => h.Region);

			var viewModel = await location.Select(h => new LocationsViewModel
			{
				LocationID = h.LocationID,
				LocationName = h.LocationName,
				LocationAddr = h.LocationAddr,
				LocationLat = h.LocationLat,
				LocationLng = h.LocationLng,
				LocationDesc = h.LocationDesc,
				LocationPrice = h.LocationPrice,
				DistrictName = h.District.DistrictName,
				RegionName = h.Region.RegionName,
				Rating = h.Rating,
				Views = h.Views,
				LocationCode = h.LocationCode,
				IsActive = h.IsActive,
				Picture = h.LocationPics.Select(p => p.Picture).ToList(),
				Keywords = h.LocationKeywords.
					Select(k => k.Keyword.KeywordID).ToList()
			}).ToListAsync();

			 return View(viewModel);
        }

        // GET: TravelManagement/Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locations = await _context.Locations
                .Include(l => l.District)
                .Include(l => l.Region)

                .FirstOrDefaultAsync(m => m.LocationID == id);

            if (locations == null)
            {
                return NotFound();
            }

            return View(locations);
        }

        // GET: TravelManagement/Locations/Create
        public IActionResult Create()
        {
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID");
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID");
            return View();
        }

        // POST: TravelManagement/Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationID,LocationName,LocationAddr,LocationLat,LocationLng,LocationDesc,LocationPrice,RegionID,DistrictID,Rating,Views,LocationCode,IsActive")] Locations locations)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", locations.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", locations.RegionID);
            return View(locations);
        }

        // GET: TravelManagement/Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locations = await _context.Locations.FindAsync(id);
            if (locations == null)
            {
                return NotFound();
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", locations.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", locations.RegionID);
            return View(locations);
        }

        // POST: TravelManagement/Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationID,LocationName,LocationAddr,LocationLat,LocationLng,LocationDesc,LocationPrice,RegionID,DistrictID,Rating,Views,LocationCode,IsActive")] Locations locations)
        {
            if (id != locations.LocationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationsExists(locations.LocationID))
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
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", locations.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", locations.RegionID);
            return View(locations);
        }

        // GET: TravelManagement/Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locations = await _context.Locations
                .Include(l => l.District)
                .Include(l => l.Region)
                .FirstOrDefaultAsync(m => m.LocationID == id);
            if (locations == null)
            {
                return NotFound();
            }

            return View(locations);
        }

        // POST: TravelManagement/Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var locations = await _context.Locations.FindAsync(id);
            if (locations != null)
            {
                _context.Locations.Remove(locations);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationsExists(int id)
        {
            return _context.Locations.Any(e => e.LocationID == id);
        }
    }
}
