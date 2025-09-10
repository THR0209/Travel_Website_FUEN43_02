using Cat_Paw_Footprint.Areas.Admin.ViewModel;
using Cat_Paw_Footprint.Data;
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
                    .Select(n => new NewsIndexViewModel
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
                .Include(n => n.Employee) // 如果要抓員工姓名
                .FirstOrDefaultAsync(m => m.NewsID == id);

            if (newsTable == null)
            {
                return NotFound();
            }

            // 映射成 ViewModel
            var viewModel = new NewsDetailViewModel
            {
                NewsID = newsTable.NewsID,
                NewsTitle = newsTable.NewsTitle,
                NewsContent = newsTable.NewsContent,
                PublishTime = newsTable.PublishTime,
                ExpireTime = newsTable.ExpireTime,
                IsActive = newsTable.IsActive?? false, // 確保不是 null
                CreateTime = newsTable.CreateTime,
                UpdateTime = newsTable.UpdateTime,
                EmployeeName = newsTable.Employee?.EmployeeName
            };

            return View(viewModel);
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

            // 映射成 ViewModel
            var viewModel = new NewsEditViewModel
            {
                NewsID = newsTable.NewsID,
                NewsTitle = newsTable.NewsTitle,
                NewsContent = newsTable.NewsContent,
                PublishTime = newsTable.PublishTime,
                ExpireTime = newsTable.ExpireTime,
                IsActive = newsTable.IsActive?? false, // 確保不是 null
                EmployeeID = newsTable.EmployeeID,
                EmployeeName = newsTable.Employee?.EmployeeName,
            };

            // 下拉選單 (員工清單)
            ViewData["EmployeeID"] = new SelectList(
                _context.EmployeeProfile,
                "EmployeeID",
                "EmployeeName", // 如果你有姓名欄位可以顯示
                newsTable.EmployeeID
            );

            return View(viewModel);
        }

        // POST: Admin/NewsTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewsEditViewModel viewModel)
        {
            if (id != viewModel.NewsID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 找到要更新的資料
                    var newsTable = await _context.NewsTable.FindAsync(id);
                    if (newsTable == null)
                    {
                        return NotFound();
                    }

                    // 將 ViewModel 的資料更新回 Entity
                    newsTable.NewsTitle = viewModel.NewsTitle;
                    newsTable.NewsContent = viewModel.NewsContent;
                    newsTable.PublishTime = viewModel.PublishTime;
                    newsTable.ExpireTime = viewModel.ExpireTime;
                    newsTable.IsActive = viewModel.IsActive;
                    newsTable.UpdateTime = DateTime.Now; // 通常編輯會更新 UpdateTime
                    newsTable.EmployeeID = viewModel.EmployeeID;

                    _context.Update(newsTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.NewsTable.Any(e => e.NewsID == viewModel.NewsID))
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

            // 如果 ModelState 驗證失敗，要重建下拉選單
            ViewData["EmployeeID"] = new SelectList(
                _context.EmployeeProfile,
                "EmployeeID",
                "EmployeeName",
                viewModel.EmployeeID
            );

            return View(viewModel);
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
