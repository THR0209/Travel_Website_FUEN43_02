using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Data;

namespace Cat_Paw_Footprint.Areas.Order.Controllers
{
    [Area("Order")]
    public class CustomerOrdersController : Controller
    {
        private readonly webtravel2Context _context;

        public CustomerOrdersController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: Order/CustomerOrders
        public async Task<IActionResult> Index()
        {
            var webtravel2Context = _context.CustomerOrders.Include(c => c.Customer).Include(c => c.OrderStatus).Include(c => c.Product);
            return View(await webtravel2Context.ToListAsync());
        }

        // GET: Order/CustomerOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerOrders = await _context.CustomerOrders
                .Include(c => c.Customer)
                .Include(c => c.OrderStatus)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (customerOrders == null)
            {
                return NotFound();
            }

            return View(customerOrders);
        }

        // GET: Order/CustomerOrders/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.CustomerProfile, "CustomerID", "CustomerID");
            ViewData["OrderStatusID"] = new SelectList(_context.OrderStatus, "OrderStatusID", "OrderStatusID");
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID");
            return View();
        }

        // POST: Order/CustomerOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,ProductID,OrderID,OrderStatusID,TotalAmount,CreateTime,UpdateTime")] CustomerOrders customerOrders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerOrders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.CustomerProfile, "CustomerID", "CustomerID", customerOrders.CustomerID);
            ViewData["OrderStatusID"] = new SelectList(_context.OrderStatus, "OrderStatusID", "OrderStatusID", customerOrders.OrderStatusID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", customerOrders.ProductID);
            return View(customerOrders);
        }

        // GET: Order/CustomerOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerOrders = await _context.CustomerOrders.FindAsync(id);
            if (customerOrders == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.CustomerProfile, "CustomerID", "CustomerID", customerOrders.CustomerID);
            ViewData["OrderStatusID"] = new SelectList(_context.OrderStatus, "OrderStatusID", "OrderStatusID", customerOrders.OrderStatusID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", customerOrders.ProductID);
            return View(customerOrders);
        }

        // POST: Order/CustomerOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,ProductID,OrderID,OrderStatusID,TotalAmount,CreateTime,UpdateTime")] CustomerOrders customerOrders)
        {
            if (id != customerOrders.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerOrders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerOrdersExists(customerOrders.OrderID))
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
            ViewData["CustomerID"] = new SelectList(_context.CustomerProfile, "CustomerID", "CustomerID", customerOrders.CustomerID);
            ViewData["OrderStatusID"] = new SelectList(_context.OrderStatus, "OrderStatusID", "OrderStatusID", customerOrders.OrderStatusID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", customerOrders.ProductID);
            return View(customerOrders);
        }

        // GET: Order/CustomerOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerOrders = await _context.CustomerOrders
                .Include(c => c.Customer)
                .Include(c => c.OrderStatus)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (customerOrders == null)
            {
                return NotFound();
            }

            return View(customerOrders);
        }

        // POST: Order/CustomerOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerOrders = await _context.CustomerOrders.FindAsync(id);
            if (customerOrders != null)
            {
                _context.CustomerOrders.Remove(customerOrders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerOrdersExists(int id)
        {
            return _context.CustomerOrders.Any(e => e.OrderID == id);
        }
        [HttpGet]
        [Route("Order/CustomerOrders/by-product/{productId:int}")]
        public async Task<IActionResult> ByProduct(int productId)
        {
            var q = _context.CustomerOrders
                            .AsNoTracking()
                            .Include(o => o.Customer)
                            .Include(o => o.OrderStatus)
                            .Include(o => o.Product)
                            .Where(o => o.ProductID == productId)
                            .OrderByDescending(o => o.CreateTime);

            var orders = await q.Select(o => new {
                id = o.OrderID,
                orderCode = "ORD-" + o.CreateTime.Value.ToString("yyyyMMdd" + "-" + $"{o.OrderID}"),   
                totalAmount = o.TotalAmount,
                createTime = o.CreateTime,
                updateTime = o.UpdateTime,
                customer = o.Customer != null? (o.Customer.CustomerName ?? o.Customer.CustomerID.ToString()) : "",
                status = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
                product = o.Product != null ? o.Product.ProductID : 0
            }).ToListAsync();

            return Ok(new
            {
                orders,
                kpi = new
                {
                    total = orders.Count,
                    totalAmount = orders.Sum(x => (decimal?)x.totalAmount ?? 0m)
                }
            });
        }
        [HttpGet]
        [Route("Order/CustomerOrders/get/{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var o = await _context.CustomerOrders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.OrderStatus)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.OrderID == id);

            if (o == null) return NotFound();

            return Ok(new
            {
                id = o.OrderID,
                customerId = o.CustomerID,
                productId = o.ProductID,
                orderStatusId = o.OrderStatusID,
                totalAmount = o.TotalAmount,
                createTime = o.CreateTime,
                updateTime = o.UpdateTime,
            });
        }
    }
}
