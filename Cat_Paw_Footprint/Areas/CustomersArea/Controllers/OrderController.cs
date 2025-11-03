using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
    [Area("CustomersArea")]
    [Route("CustomersArea/[controller]")]
    [Authorize(AuthenticationSchemes = "CustomerAuth")]
    public class OrdersController : Controller
	{
		private readonly webtravel2Context _db;
        private readonly ICustomerSupportTicketsService _ticketSvc;
        public OrdersController(webtravel2Context db, ICustomerSupportTicketsService ticketSvc)
        {
            _db = db;
            _ticketSvc = ticketSvc;
        }
        private int CurrentCustomerId =>
        int.TryParse(User.FindFirst("CustomerId")?.Value, out var id) ? id : 0;
        [AllowAnonymous]
        [HttpGet("")]            // 對應 /CustomersArea/Cart
		[HttpGet("Index")]       // 也讓 /CustomersArea/Cart/Index 可用（加這行！）
		public IActionResult Index() => View();

        // ?customerId=123
        [HttpGet("api")]
        public async Task<IActionResult> MyOrders()
        {
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();

            var list = await _db.CustomerOrders
                .AsNoTracking()
                .Include(x => x.OrderStatus)
                .Include(x => x.Product)
                .Include(x => x.CustomerProfile)
                .Where(x => x.CustomerID == cid)
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
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();

            var o = await _db.CustomerOrders
                .AsNoTracking()
                .Include(x => x.OrderStatus)
                .Include(x => x.Product)
                .Include(x => x.CustomerProfile)
                .FirstOrDefaultAsync(x => x.OrderID == id && x.CustomerID == cid); // ★ 防止看別人的

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

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromForm] int orderId, [FromForm] string reason)
        {
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();
            try
            {
				var o = await _db.CustomerOrders
										 .AsNoTracking()
										 .FirstOrDefaultAsync(x => x.OrderID == orderId && x.CustomerID == cid); if (o == null) return NotFound(new { ok = false, error = "找不到訂單" });
				if (o == null) return NotFound(new { ok = false, error = "找不到訂單" });

				// 產生顯示用的訂單編號（若資料表沒有 OrderCode 欄位）
				string orderCode = "ORD-" + o.CreateTime.Value.ToString("yyyyMMdd" + "-" + $"{o.OrderID}");

				// ★ 指派客服：挑工單未結案數最少的客服
				var assignedEmp = await _db.Employees
					.Include(e => e.Role)
					.Where(e => e.Role.RoleName == "CustomerService")
					.OrderBy(e => _db.CustomerSupportTickets
					.Count(t => t.EmployeeID == e.EmployeeID && t.StatusID != 3)) // 3=已結案
					.FirstOrDefaultAsync();
				if (assignedEmp == null)
					return StatusCode(500, new { ok = false, error = "找不到客服人員" });

				// 建工單（用你團隊的 service）
				await _ticketSvc.AddAsync(new CustomerSupportTicketViewModel
                {
                    CustomerID = cid,
                    EmployeeID = assignedEmp.EmployeeID,    // 沒指定就 null 或排程分派
					Subject = $"取消訂單申請 #{orderCode}",
                    TicketTypeID = 1,              // ← 請依你們的對照表給一個有效值
                    Description = reason,
                    StatusID = 1,                  // ← 例如 1=新建立
                    PriorityID = 2,                // ← 給個預設優先權
                    CreateTime = DateTime.Now,
                    TicketCode = $"T{DateTime.Now:yyyyMMddHH}"
                });

                return Ok(new { ok = true });
            }

            catch (Exception ex)
            {
                return BadRequest(new { ok = false, error = ex.Message });
            }
        }
    }
}
