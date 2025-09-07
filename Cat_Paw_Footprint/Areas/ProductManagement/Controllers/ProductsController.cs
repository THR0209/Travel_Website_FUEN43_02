using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Data;

namespace Cat_Paw_Footprint.Areas.ProductManagement.Controllers
{
    [Area("ProductManagement")]
    public class ProductsController : Controller
    {
        private readonly webtravel2Context _context;

        public ProductsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: ProductManagement/Products
        public async Task<IActionResult> Index()
        {
            var webtravel2Context = _context.Products.Include(p => p.Region);
            return View(await webtravel2Context.ToListAsync());
        }

        // GET: ProductManagement/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Region)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: ProductManagement/Products/Create
        public IActionResult Create()
        {
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID");
            return View();
        }

        // POST: ProductManagement/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,RegionID,ProductDesc,ProductPrice,ProductNote,StartDate,EndDate,MaxPeople,ProductImage,ProductCode")] Products products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", products.RegionID);
            return View(products);
        }

        // GET: ProductManagement/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", products.RegionID);
            return View(products);
        }

        // POST: ProductManagement/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,RegionID,ProductDesc,ProductPrice,ProductNote,StartDate,EndDate,MaxPeople,ProductImage,ProductCode")] Products products)
        {
            if (id != products.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductID))
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
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionID", products.RegionID);
            return View(products);
        }

        // GET: ProductManagement/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Region)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: ProductManagement/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products != null)
            {
                _context.Products.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
