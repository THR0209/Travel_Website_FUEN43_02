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
    public class RestaurantsController : Controller
    {
        private readonly webtravel2Context _context;

        public RestaurantsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: TravelManagement/Restaurants
        public async Task<IActionResult> Index()
        {
            var webtravel2Context = _context.Restaurants.Include(r => r.District).Include(r => r.Region);
            return View(await webtravel2Context.ToListAsync());
        }

        // GET: TravelManagement/Restaurants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurants = await _context.Restaurants
                .Include(r => r.District)
                .Include(r => r.Region)
                .FirstOrDefaultAsync(m => m.RestaurantID == id);
            if (restaurants == null)
            {
                return NotFound();
            }

            return View(restaurants);
        }

        // GET: TravelManagement/Restaurants/Create
        public IActionResult Create()
        {
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID");
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID");
            return View();
        }

        // POST: TravelManagement/Restaurants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RestaurantID,RestaurantName,RestaurantAddr,RestaurantLat,RestaurantLng,RestaurantDesc,RegionID,DistrictID,Rating,Views,IsActive,RestaurantCode")] Restaurants restaurants)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurants);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", restaurants.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", restaurants.RegionID);
            return View(restaurants);
        }

        // GET: TravelManagement/Restaurants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurants = await _context.Restaurants.FindAsync(id);
            if (restaurants == null)
            {
                return NotFound();
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", restaurants.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", restaurants.RegionID);
            return View(restaurants);
        }

        // POST: TravelManagement/Restaurants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RestaurantID,RestaurantName,RestaurantAddr,RestaurantLat,RestaurantLng,RestaurantDesc,RegionID,DistrictID,Rating,Views,IsActive,RestaurantCode")] Restaurants restaurants)
        {
            if (id != restaurants.RestaurantID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurants);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantsExists(restaurants.RestaurantID))
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
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictID", restaurants.DistrictID);
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", restaurants.RegionID);
            return View(restaurants);
        }

        // GET: TravelManagement/Restaurants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurants = await _context.Restaurants
                .Include(r => r.District)
                .Include(r => r.Region)
                .FirstOrDefaultAsync(m => m.RestaurantID == id);
            if (restaurants == null)
            {
                return NotFound();
            }

            return View(restaurants);
        }

        // POST: TravelManagement/Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurants = await _context.Restaurants.FindAsync(id);
            if (restaurants != null)
            {
                _context.Restaurants.Remove(restaurants);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantsExists(int id)
        {
            return _context.Restaurants.Any(e => e.RestaurantID == id);
        }
    }
}
