using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomersArea.OrderControllers
{
    [Area("CustomersArea")]
    [Route("CustomersArea/[controller]")]
    public class OrdersController : Controller
    {
        private readonly webtravel2Context _db;
        public OrdersController(webtravel2Context db) => _db = db;

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index() => View();
        [HttpGet("api")]
        public async Task<IActionResult> MyOrders([FromQuery] int customerId)
        {
            var list = await _db.CustomerOrders
                .AsNoTracking()
                .Include(x => x.OrderStatus)
                .Include(x => x.Product)
                .Where(x => x.CustomerID == customerId)
                .OrderByDescending(x => x.CreateTime)
                .Select(o => new {
                    id = o.OrderID,
                    orderCode = "ORD-" + (o.CreateTime.HasValue ? o.CreateTime.Value.ToString("yyyyMMdd-HHmmss") : "NA") + "-" + o.OrderID,
                    productName = o.Product != null ? o.Product.ProductName : $"商品 {o.ProductID}",
                    amount = o.TotalAmount ?? 0,
                    status = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
                    createTime = o.CreateTime
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}