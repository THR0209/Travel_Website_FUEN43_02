using Cat_Paw_Footprint.Areas.CustomerService.Services; // 匯入客戶服務工單相關業務邏輯服務介面
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel; // 匯入工單 ViewModel
using Microsoft.AspNetCore.Authorization; // 匯入身份驗證/授權相關功能
using Cat_Paw_Footprint.Data; // 匯入資料庫 DbContext
using Cat_Paw_Footprint.Models; // 匯入資料庫模型
using Microsoft.AspNetCore.Mvc; // 匯入 MVC 控制器相關功能
using Microsoft.EntityFrameworkCore; // 匯入 Entity Framework Core

namespace Cat_Paw_Footprint.Areas.CustomerService.Controllers
{
	// 設定此 Controller 屬於 CustomerService 區域
	[Area("CustomerService")]
	// 需員工身份驗證且符合 AreaCustomerService 授權政策
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaCustomerService")]
	// 路由格式：CustomerService/[controller]/[action]
	[Route("CustomerService/[controller]/[action]")]
	public class CustomerSupportTicketsController : Controller
	{
		// 注入工單服務
		private readonly ICustomerSupportTicketsService _service;
		// 注入資料庫 DbContext
		private readonly webtravel2Context _context;

		// 建構式注入服務與 DbContext
		public CustomerSupportTicketsController(ICustomerSupportTicketsService service, webtravel2Context context)
		{
			_service = service;
			_context = context;
		}

		/// <summary>
		/// 主頁：顯示目前員工的所有工單
		/// </summary>
		public async Task<IActionResult> Index()
		{
			// 取得目前登入員工 ID
			var empId = User.FindFirst("EmployeeID")?.Value;
			// 篩選屬於此員工的工單
			var tickets = (await _service.GetAllAsync())
				.Where(t => t.EmployeeID?.ToString() == empId);
			return View(tickets);
		}

		/// <summary>
		/// 取得所有工單詳細資料
		/// GET: /CustomerService/CustomerSupportTickets/GetTickets
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetTickets()
		{
			// 使用 EF 取得工單及關聯資料
			var tickets = await _context.CustomerSupportTickets
				.Include(t => t.Employee)
					.ThenInclude(e => e.EmployeeProfile)
				.Include(t => t.Customer)
				.Include(t => t.TicketType)
				.Include(t => t.Status)
				.Include(t => t.Priority)
				.Select(t => new
				{
					ticketID = t.TicketID != null ? t.TicketID : 0,
					ticketCode = t.TicketCode ?? "",
					customerID = t.CustomerID != null ? t.CustomerID : 0,
					customerName = t.Customer != null && t.Customer.CustomerName != null ? t.Customer.CustomerName : "",
					customerEmail = t.Customer != null && t.Customer.Email != null ? t.Customer.Email : "", // 顯示客戶 Email
					employeeID = t.EmployeeID != null ? t.EmployeeID : 0,
					employeeName = t.Employee != null && t.Employee.EmployeeProfile != null && t.Employee.EmployeeProfile.EmployeeName != null
						? t.Employee.EmployeeProfile.EmployeeName : "",
					subject = t.Subject ?? "",
					description = t.Description ?? "",
					ticketTypeID = t.TicketTypeID != null ? t.TicketTypeID : 0,
					ticketTypeName = t.TicketType != null && t.TicketType.TicketTypeName != null ? t.TicketType.TicketTypeName : "",
					statusID = t.StatusID != null ? t.StatusID : 0,
					statusName = t.Status != null && t.Status.StatusDesc != null ? t.Status.StatusDesc : "",
					priorityID = t.PriorityID != null ? t.PriorityID : 0,
					priorityName = t.Priority != null && t.Priority.PriorityDesc != null ? t.Priority.PriorityDesc : "",
					createTime = t.CreateTime.HasValue ? t.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""
				})
				.ToListAsync();

			return Json(tickets); // 回傳 JSON 格式
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
				employeeName = t.EmployeeName ?? "",
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
		/// 新增工單，處理人員自動取目前登入者
		/// POST: /CustomerService/CustomerSupportTickets/CreateTicket
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> CreateTicket([FromBody] CustomerSupportTicketViewModel vm)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			// 取得目前登入員工 ID
			var empIDStr = User.FindFirst("EmployeeID")?.Value;
			if (string.IsNullOrEmpty(empIDStr) || !int.TryParse(empIDStr, out int employeeID))
				return BadRequest("無法取得登入員工ID");

			// 產生 TicketCode (格式: CST+年月日+當日流水號4碼)
			var today = DateTime.Now.Date;
			var countToday = await _context.CustomerSupportTickets
				.CountAsync(t => t.CreateTime.HasValue && t.CreateTime.Value.Date == today);
			var newCode = $"CST{today:yyMMdd}{(countToday + 1).ToString("D4")}";

			// 建立新工單實體
			var entity = new CustomerSupportTickets
			{
				CustomerID = vm.CustomerID,
				EmployeeID = employeeID,
				Subject = vm.Subject,
				TicketTypeID = vm.TicketTypeID,
				Description = vm.Description,
				StatusID = vm.StatusID,
				PriorityID = vm.PriorityID,
				CreateTime = DateTime.Now,
				UpdateTime = DateTime.Now,
				TicketCode = newCode
			};

			// 呼叫 Service 新增工單
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

			// 取得新增的工單完整資料
			var createdTicket = (await _service.GetAllAsync()).OrderByDescending(t => t.TicketID).FirstOrDefault();

			// 回傳 DataTables 所需欄位
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
			// 只允許修改狀態及優先度
			var ticket = await _service.GetByIdAsync(vm.TicketID);
			if (ticket == null) return NotFound();

			ticket.StatusID = vm.StatusID;
			ticket.PriorityID = vm.PriorityID;
			await _service.UpdateAsync(ticket);

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

				// 檢查是否有關聯回饋資料
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
			// 顯示連線字串（除錯用）
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