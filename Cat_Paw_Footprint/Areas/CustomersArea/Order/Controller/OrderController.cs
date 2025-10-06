using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;

namespace Cat_Paw_Footprint.Areas.CustomerArea.Order.Controllers
{
    [Area("CustomerArea")]
    [Route("CustomerArea/Order/[controller]")]
    public class OrdersController : Controller
    {
        private readonly webtravel2Context _db;
        public OrdersController(webtravel2Context db) => _db = db;

        [HttpGet("")]
        public IActionResult Index()
        => View("~/Areas/CustomerArea/Order/Views/Orders/Index.cshtml");

        // 顧客自己的訂單（?customerId=123）
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
                    productId = o.ProductID,
                    productName = o.Product != null ? o.Product.ProductName : $"商品 {o.ProductID}",
                    amount = o.TotalAmount ?? 0,
                    status = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
                    createTime = o.CreateTime,
                    updateTime = o.UpdateTime
                })
                .ToListAsync();

            return Ok(list);
        }

        // 單筆明細
        [HttpGet("api/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            var o = await _db.CustomerOrders
                .AsNoTracking()
                .Include(x => x.OrderStatus)
                .Include(x => x.Product)
                .Include(x => x.CustomerProfile)
                .FirstOrDefaultAsync(x => x.OrderID == id);

            if (o == null) return NotFound();

            return Ok(new
            {
                id = o.OrderID,
                orderCode = "ORD-" + (o.CreateTime.HasValue ? o.CreateTime.Value.ToString("yyyyMMdd-HHmmss") : "NA") + "-" + o.OrderID,
                product = o.Product?.ProductName ?? $"商品 {o.ProductID}",
                amount = o.TotalAmount ?? 0,
                status = o.OrderStatus?.StatusDesc ?? "",
                customer = o.CustomerProfile?.CustomerName ?? $"客戶 {o.CustomerID}",
                email = o.CustomerProfile?.Email ?? "",
                createTime = o.CreateTime,
                updateTime = o.UpdateTime
            });
        }
    }
}
