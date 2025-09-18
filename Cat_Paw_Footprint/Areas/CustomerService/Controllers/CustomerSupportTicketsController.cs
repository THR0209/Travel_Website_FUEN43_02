using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomerService.Controllers
{
	[Area("CustomerService")]
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaCustomerService")]
	[Route("CustomerService/[controller]/[action]")]
	public class CustomerSupportTicketsController : Controller
	{
		private readonly ICustomerSupportTicketsService _service;
		private readonly webtravel2Context _context;

		public CustomerSupportTicketsController(ICustomerSupportTicketsService service, webtravel2Context context)
		{
			_service = service;
			_context = context;
		}

		// 主頁
		public IActionResult Index() => View();

		// 取得所有工單資料
		[HttpGet]
		[HttpGet]
		public async Task<IActionResult> GetTickets()
		{
			var tickets = await _context.CustomerSupportTickets
				.Include(t => t.Employee)
					.ThenInclude(e => e.EmployeeProfile)
				.Include(t => t.Customer)
				.Include(t => t.TicketType)
				.Include(t => t.Status)
				.Include(t => t.Priority)
				.Select(t => new
				{
					ticketID = t.TicketID != null ? t.TicketID : 0, // 防 null
					ticketCode = t.TicketCode ?? "",
					customerID = t.CustomerID != null ? t.CustomerID : 0,
					customerName = t.Customer != null && t.Customer.CustomerName != null ? t.Customer.CustomerName : "",
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

			return Json(tickets);
		}

		// 取得指定工單
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

		// 新增工單（處理人員自動取目前登入者）
		[HttpPost]
		public async Task<IActionResult> CreateTicket([FromBody] CustomerSupportTicketViewModel vm)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			// 取得目前登入員工ID
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

			// 回傳 DataTables 所需的所有欄位
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

		// 編輯工單
		[HttpPost]
		public async Task<IActionResult> EditTicket([FromBody] CustomerSupportTicketViewModel vm)
		{
			if (!await _service.ExistsAsync(vm.TicketID)) return NotFound();
			await _service.UpdateAsync(vm);
			return Json(new { success = true });
		}

		// 刪除工單
		[HttpPost]
		public async Task<IActionResult> DeleteTicket([FromBody] int id)
		{
			if (!await _service.ExistsAsync(id)) return NotFound();
			await _service.DeleteAsync(id);
			return Json(new { success = true });
		}

		// 取得所有下拉選單資料
		[HttpGet]
		public async Task<IActionResult> GetDropdowns()
		{
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

		// 客戶 autocomplete API
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