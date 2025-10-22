using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Route("CustomersArea/[controller]")]
	public class OrdersController : Controller
	{
		private readonly webtravel2Context _db;
		public OrdersController(webtravel2Context db) => _db = db;

		[HttpGet("")]            // 對應 /CustomersArea/Cart
		[HttpGet("Index")]       // 也讓 /CustomersArea/Cart/Index 可用（加這行！）
		public IActionResult Index() => View();

		// ?customerId=123
		[HttpGet("api")]
		public async Task<IActionResult> MyOrders([FromQuery] int customerId)
		{
			var list = await _db.CustomerOrders
				.AsNoTracking()
				.Include(x => x.OrderStatus)
				.Include(x => x.Product)
				.Include(x => x.CustomerProfile)
				.Where(x => x.CustomerID == customerId)
				.OrderByDescending(x => x.CreateTime)
				.Select(o => new {
					id = o.OrderID,
					orderCode = "ORD-" + (o.CreateTime.HasValue ? o.CreateTime.Value.ToString("yyyyMMdd-HHmmss") : "NA") + "-" + o.OrderID,
					product = o.Product != null ? o.Product.ProductName : $"商品 {o.ProductID}",
					amount = o.TotalAmount ?? 0,
					status = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
					createTime = o.CreateTime,
					updateTime = o.UpdateTime,
					customerName = o.CustomerProfile != null ? o.CustomerProfile.CustomerName : null
				})
				.ToListAsync();

			return Ok(list);
		}

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
