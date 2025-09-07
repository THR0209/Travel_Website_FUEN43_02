using Cat_Paw_Footprint.Areas.Admin.ViewModel;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsTablesController : Controller
    {
        private readonly webtravel2Context _context;

        public NewsTablesController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: Admin/NewsTables
        public async Task<IActionResult> Index()
        {
            var data = await _context.NewsTable
                .Include(n => n.Employee)
                    .Select(n => new NewsReadViewModel
                        {
                            NewsID = n.NewsID,
                            NewsTitle = n.NewsTitle,
                            PublishTime = n.PublishTime,
                            ExpireTime = n.ExpireTime,
                            IsActive = n.IsActive,
                            UpdateTime = n.UpdateTime,

                            // 這裡要看你需求
                            // 如果要顯示員工名稱 → 改成 n.Employee.Name (string)
                            // 如果只是存 EmployeeId → 就用 n.EmployeeId
                            EmployeeName = n.Employee.EmployeeName,
                            Employee = n.Employee
                         }).ToListAsync();

            return View(data);
        }

        // GET: Admin/NewsTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsTable = await _context.NewsTable
                .Include(n => n.Employee)
                .FirstOrDefaultAsync(m => m.NewsID == id);
            if (newsTable == null)
            {
                return NotFound();
            }

            return View(newsTable);
        }

        // GET: Admin/NewsTables/Create
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.EmployeeProfile, "EmployeeID", "EmployeeID");
            return View();
        }

        // POST: Admin/NewsTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NewsID,NewsTitle,NewsContent,PublishTime,ExpireTime,IsActive,CreateTime,UpdateTime,EmployeeID")] NewsTable newsTable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newsTable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.EmployeeProfile, "EmployeeID", "EmployeeID", newsTable.EmployeeID);
            return View(newsTable);
        }

        // GET: Admin/NewsTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsTable = await _context.NewsTable.FindAsync(id);
            if (newsTable == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.EmployeeProfile, "EmployeeID", "EmployeeID", newsTable.EmployeeID);
            return View(newsTable);
        }

        // POST: Admin/NewsTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NewsID,NewsTitle,NewsContent,PublishTime,ExpireTime,IsActive,CreateTime,UpdateTime,EmployeeID")] NewsTable newsTable)
        {
            if (id != newsTable.NewsID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsTableExists(newsTable.NewsID))
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
            ViewData["EmployeeID"] = new SelectList(_context.EmployeeProfile, "EmployeeID", "EmployeeID", newsTable.EmployeeID);
            return View(newsTable);
        }

        // GET: Admin/NewsTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsTable = await _context.NewsTable
                .Include(n => n.Employee)
                .FirstOrDefaultAsync(m => m.NewsID == id);
            if (newsTable == null)
            {
                return NotFound();
            }

            return View(newsTable);
        }

        // POST: Admin/NewsTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var newsTable = await _context.NewsTable.FindAsync(id);
            if (newsTable != null)
            {
                _context.NewsTable.Remove(newsTable);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsTableExists(int id)
        {
            return _context.NewsTable.Any(e => e.NewsID == id);
        }
    }
}
