using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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
            var webtravel2Context = _context.CustomerOrders.Include(c => c.CustomerProfile).Include(c => c.CustomerProfile).Include(c => c.OrderStatus).Include(c => c.Product);
            return View(await webtravel2Context.ToListAsync());
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
                            .Include(o => o.CustomerProfile)
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
                customer = o.CustomerProfile != null? (o.CustomerProfile.CustomerName ?? o.CustomerProfile.CustomerID.ToString()) : "",
                status = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
				customerEmail = o.CustomerProfile != null ? o.CustomerProfile.Email : "",
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
                .Include(x => x.CustomerProfile)
                .Include(x => x.OrderStatus)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.OrderID == id);

            if (o == null) return NotFound();

            return Ok(new
            {
                id = o.OrderID,
                orderCode = "ORD-" + o.CreateTime.Value.ToString("yyyyMMdd" + "-" + $"{o.OrderID}"),
                status = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
                customerId = o.CustomerID,
                productId = o.ProductID,
                orderStatusId = o.OrderStatusID,
				customerEmail = o.CustomerProfile?.Email,
				totalAmount = o.TotalAmount,
                createTime = o.CreateTime,
                updateTime = o.UpdateTime,
            });
        }

        [HttpGet("Order/CustomerOrders/Export")]
        public async Task<IActionResult> Export(DateTime? start, DateTime? end)
        {

            // 基礎查詢（連同關聯）
            var q = _context.CustomerOrders
                .AsNoTracking()
                .Include(x => x.CustomerProfile)
                .Include(x => x.Product)
                .Include(x => x.OrderStatus)
                .AsQueryable();

            // 時間範圍篩選（使用 CreateTime）
            if (start.HasValue) q = q.Where(o => o.CreateTime >= start);
            if (end.HasValue) q = q.Where(o => o.CreateTime <= end);


            var data = await q
                .OrderBy(o => o.CreateTime)
                .Select(o => new
                {
                    訂單編號 = "ORD-" + (o.CreateTime.HasValue ? o.CreateTime.Value.ToString("yyyyMMdd") : "NA") + "-" + o.OrderID,
                    建立時間 = o.CreateTime,
                    更新時間 = o.UpdateTime,
                    客戶 = o.CustomerProfile != null ? o.CustomerProfile.CustomerName : o.CustomerID.ToString(),
                    客戶Email = o.CustomerProfile != null ? o.CustomerProfile.Email : "",
                    商品 = o.Product != null ? o.Product.ProductName : o.ProductID.ToString(),
                    狀態 = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
                    總金額 = o.TotalAmount ?? 0
                })
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Orders");
            // 一行丟整個匿名型別清單會自動生表頭
            ws.Cell(1, 1).InsertTable(data, "Orders", true);
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;

            string fname = $"orders_{(start.HasValue ? start.Value.ToString("yyyyMMdd") : "all")}_{(end.HasValue ? end.Value.ToString("yyyyMMdd") : "all")}.xlsx";
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fname);
        }
    }
}
