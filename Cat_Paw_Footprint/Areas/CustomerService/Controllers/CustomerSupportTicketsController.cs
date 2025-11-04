using Cat_Paw_Footprint.Areas.CustomerService.Services; // 匯入客戶服務工單相關業務邏輯服務介面
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel; // 匯入工單 ViewModel
using Cat_Paw_Footprint.Areas.Notification.Services; // 匯入通知服務介面
using Cat_Paw_Footprint.Data; // 匯入資料庫 DbContext
using Cat_Paw_Footprint.Models; // 匯入資料庫模型
using Cat_Paw_Footprint.Services;
using Microsoft.AspNetCore.Authorization; // 匯入身份驗證/授權相關功能
using Microsoft.AspNetCore.Mvc; // 匯入 MVC 控制器相關功能
using Microsoft.AspNetCore.SignalR; // 匯入 SignalR 功能
using Microsoft.EntityFrameworkCore; // 匯入 Entity Framework Core

namespace Cat_Paw_Footprint.Areas.CustomerService.Controllers
{
	// 設定此 Controller 屬於 CustomerService 區域
	[Area("CustomerService")]
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaCustomerService")]
	[Route("CustomerService/[controller]/[action]")]
	public class CustomerSupportTicketsController : Controller
	{
		private readonly ICustomerSupportTicketsService _service;
		private readonly webtravel2Context _context;
		private readonly INotificationTriggerService _notifTrigger;
		private readonly IHubContext<TicketChatHub> _hubContext;

		public CustomerSupportTicketsController(ICustomerSupportTicketsService service, webtravel2Context context,
	INotificationTriggerService notifTrigger, IHubContext<TicketChatHub> hubContext)
		{
			_service = service;
			_context = context;
			_notifTrigger = notifTrigger;
			_hubContext = hubContext;
		}

		/// <summary>
		/// 主頁：顯示目前員工的所有工單（SuperAdmin 可查看全部）
		/// </summary>
		public async Task<IActionResult> Index()
		{
			var empId = User.FindFirst("EmployeeID")?.Value;
			var roleName = User.FindFirst("RoleName")?.Value ?? "";
			var allTickets = await _service.GetAllAsync();

			IEnumerable<CustomerSupportTicketViewModel> tickets;

			// 🔹 若為 SuperAdmin，查看所有工單；否則只看自己負責的
			if (roleName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
			{
				tickets = allTickets;
			}
			else
			{
				tickets = allTickets.Where(t => t.EmployeeID?.ToString() == empId);
			}

			return View(tickets);
		}

		/// <summary>
		/// 取得所有工單詳細資料
		/// GET: /CustomerService/CustomerSupportTickets/GetTickets
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetTickets()
		{
			var tickets = await _context.CustomerSupportTickets
				.Include(t => t.Employee).ThenInclude(e => e.EmployeeProfile)
				.Include(t => t.Customer)
				.Include(t => t.TicketType)
				.Include(t => t.Status)
				.Include(t => t.Priority)
				.Select(t => new
				{
					ticketID = t.TicketID,
					ticketCode = t.TicketCode ?? "",
					customerID = t.CustomerID ?? 0,
					customerName = t.Customer != null ? t.Customer.CustomerName : "",
					customerEmail = t.Customer != null ? t.Customer.Email : "",
					employeeID = t.EmployeeID ?? 0,
					employeeName = (t.Employee != null && t.Employee.EmployeeProfile != null && !string.IsNullOrEmpty(t.Employee.EmployeeProfile.EmployeeName))
						? t.Employee.EmployeeProfile.EmployeeName
						: "尚未指派",
					subject = t.Subject ?? "",
					description = t.Description ?? "",
					ticketTypeID = t.TicketTypeID ?? 0,
					ticketTypeName = t.TicketType != null ? t.TicketType.TicketTypeName : "",
					statusID = t.StatusID ?? 0,
					statusName = t.Status != null ? t.Status.StatusDesc : "",
					priorityID = t.PriorityID ?? 0,
					priorityName = t.Priority != null ? t.Priority.PriorityDesc : "",
					createTime = t.CreateTime.HasValue ? t.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""
				})
				.ToListAsync();

			return Json(tickets);
		}

		/// <summary>
		/// 取得指定工單資料
		/// GET: /CustomerService/CustomerSupportTickets/GetById?id={id}
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetById(int id)
		{
			var t = await _service.GetByIdAsync(id);
			if (t == null) return NotFound();

			var ticket = new
			{
				t.TicketID,
				ticketCode = t.TicketCode ?? "",
				t.CustomerID,
				customerName = t.CustomerName ?? "",
				t.EmployeeID,
				employeeName = !string.IsNullOrEmpty(t.EmployeeName) ? t.EmployeeName : "尚未指派",
				t.Subject,
				t.Description,
				t.TicketTypeID,
				ticketTypeName = t.TicketTypeName ?? "",
				t.StatusID,
				statusName = t.StatusName ?? "",
				t.PriorityID,
				priorityName = t.PriorityName ?? "",
				createTime = t.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
				updateTime = t.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
			};

			return Json(ticket);
		}

		/// <summary>
		/// 新增工單，分配給待處理工單最少的 CustomerService 員工
		/// POST: /CustomerService/CustomerSupportTickets/CreateTicket
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> CreateTicket([FromBody] CustomerSupportTicketViewModel vm)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var customerServiceRoleName = "CustomerService";
			var customerServiceEmployees = await _context.Employees
				.Include(e => e.EmployeeProfile)
				.Include(e => e.Role)
				.Where(e => e.Role != null && e.Role.RoleName == customerServiceRoleName)
				.ToListAsync();

			if (!customerServiceEmployees.Any())
				return BadRequest("找不到 CustomerService 員工，請聯絡管理員");

			var pendingStatusDesc = "待處理";
			var empWithTicketCounts = customerServiceEmployees
				.Select(emp => new
				{
					emp.EmployeeID,
					PendingCount = _context.CustomerSupportTickets
						.Count(t => t.EmployeeID == emp.EmployeeID && t.Status.StatusDesc == pendingStatusDesc)
				})
				.OrderBy(x => x.PendingCount)
				.ToList();

			var selectedEmpId = empWithTicketCounts.First().EmployeeID;

			var today = DateTime.Now.Date;
			var countToday = await _context.CustomerSupportTickets
				.CountAsync(t => t.CreateTime.HasValue && t.CreateTime.Value.Date == today);
			var newCode = $"CST{today:yyMMdd}{(countToday + 1):D4}";

			var entity = new CustomerSupportTickets
			{
				CustomerID = vm.CustomerID,
				EmployeeID = selectedEmpId,
				Subject = vm.Subject,
				TicketTypeID = vm.TicketTypeID,
				Description = vm.Description,
				StatusID = vm.StatusID,
				PriorityID = vm.PriorityID,
				CreateTime = DateTime.Now,
				UpdateTime = DateTime.Now,
				TicketCode = newCode
			};

			await _service.AddAsync(new CustomerSupportTicketViewModel
			{
				CustomerID = entity.CustomerID,
				EmployeeID = entity.EmployeeID,
				Subject = entity.Subject,
				TicketTypeID = entity.TicketTypeID,
				Description = entity.Description,
				StatusID = entity.StatusID,
				PriorityID = entity.PriorityID,
				CreateTime = entity.CreateTime,
				UpdateTime = entity.UpdateTime,
				TicketCode = entity.TicketCode
			});

			var createdTicket = (await _service.GetAllAsync()).OrderByDescending(t => t.TicketID).FirstOrDefault();

			return Json(new
			{
				success = true,
				ticketID = createdTicket?.TicketID ?? 0,
				ticketCode = createdTicket?.TicketCode ?? "",
				customerName = createdTicket?.CustomerName ?? "",
				employeeName = createdTicket?.EmployeeName ?? "",
				subject = createdTicket?.Subject ?? "",
				description = createdTicket?.Description ?? "",
				ticketTypeName = createdTicket?.TicketTypeName ?? "",
				statusName = createdTicket?.StatusName ?? "",
				priorityName = createdTicket?.PriorityName ?? "",
				createTime = createdTicket?.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
			});
		}

		/// <summary>
		/// 編輯工單，只允許變更狀態與優先度
		/// POST: /CustomerService/CustomerSupportTickets/EditTicket
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> EditTicket([FromBody] CustomerSupportTicketViewModel vm)
		{
			var ticket = await _service.GetByIdAsync(vm.TicketID);
			if (ticket == null) return NotFound();

			ticket.StatusID = vm.StatusID;
			ticket.PriorityID = vm.PriorityID;
			ticket.TicketTypeID = vm.TicketTypeID;
			await _service.UpdateAsync(ticket);

			// 🔹 檢查是否變更為「已完成」
			var completedStatus = await _context.TicketStatus
				.FirstOrDefaultAsync(s => s.StatusDesc.Contains("已完成"));

			// ✅ 取得最新狀態文字（不論是哪個狀態）
			var newStatus = await _context.TicketStatus
				.Where(s => s.StatusID == vm.StatusID)
				.Select(s => s.StatusDesc)
				.FirstOrDefaultAsync();

			if (!string.IsNullOrEmpty(newStatus))
			{
				// ✅ 推播即時狀態更新給該工單聊天室群組
				await _hubContext.Clients.Group($"ticket-{vm.TicketID}")
					.SendAsync("TicketStatusChanged", vm.TicketID, newStatus);

				Console.WriteLine($"📢 已推播工單狀態更新：#{vm.TicketID} → {newStatus}");
			}

			// ✅ 若為「已完成」，觸發通知中心訊息
			if (completedStatus != null && vm.StatusID == completedStatus.StatusID)
			{
				await _notifTrigger.NotifyTicketCompletedAsync(ticket.TicketID);
			}

			return Json(new { success = true });

		}


		/// <summary>
		/// 刪除工單，會檢查是否有關聯 Feedback
		/// POST: /CustomerService/CustomerSupportTickets/DeleteTicket
		/// </summary>
		public async Task<IActionResult> DeleteTicket([FromBody] int id)
		{
			if (id <= 0) return BadRequest(new { success = false, message = "工單ID不正確" });
			try
			{
				if (!await _service.ExistsAsync(id)) return NotFound(new { success = false, message = "工單不存在" });

				bool hasFeedback = await _context.CustomerSupportFeedback.AnyAsync(f => f.TicketID == id);
				if (hasFeedback)
				{
					return BadRequest(new
					{
						success = false,
						message = "此工單有客戶回饋資料，請先刪除相關回饋再刪工單！"
					});
				}

				await _service.DeleteAsync(id);
				await _context.SaveChangesAsync();

				return Json(new { success = true });
			}
			catch (Exception ex)
			{
				var msg = ex.InnerException?.Message ?? ex.Message;
				return StatusCode(500, new { success = false, message = msg });
			}
		}

		/// <summary>
		/// 取得所有下拉選單資料 (客戶、員工、狀態、優先度、類型)
		/// GET: /CustomerService/CustomerSupportTickets/GetDropdowns
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetDropdowns()
		{
			Console.WriteLine("Using DB: " + _context.Database.GetDbConnection().ConnectionString);

			var customers = await _context.CustomerProfile
				.Select(c => new { customerID = c.CustomerID, customerName = c.CustomerName }).ToListAsync();

			var employees = await _context.Employees.Include(e => e.EmployeeProfile)
				.Select(e => new { employeeID = e.EmployeeID, employeeName = e.EmployeeProfile.EmployeeName }).ToListAsync();

			var statuses = await _context.TicketStatus
				.Select(s => new { statusID = s.StatusID, statusName = s.StatusDesc }).ToListAsync();

			var priorities = await _context.TicketPriority
				.Select(p => new { priorityID = p.PriorityID, priorityName = p.PriorityDesc }).ToListAsync();

			var types = await _context.TicketTypes
				.Select(t => new { ticketTypeID = t.TicketTypeID, ticketTypeName = t.TicketTypeName }).ToListAsync();

			return Json(new { customers, employees, statuses, priorities, types });
		}

		/// <summary>
		/// 客戶 autocomplete API
		/// GET: /CustomerService/CustomerSupportTickets/GetCustomersAutocomplete?term={term}
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetCustomersAutocomplete(string term)
		{
			term = term?.Trim().ToLower() ?? "";

			var customers = await _context.CustomerProfile
				.Where(c => term == "" || c.CustomerName.ToLower().Contains(term))
				.Select(c => new { label = c.CustomerName, value = c.CustomerID })
				.ToListAsync();

			return Json(customers);
		}
	}
}