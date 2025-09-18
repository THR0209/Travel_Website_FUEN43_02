using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
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

		public IActionResult Index() => View();

		[HttpGet]
		public async Task<IActionResult> GetTickets()
		{
			try
			{
				var tickets = await _service.GetAllAsync();
				var list = tickets.Select(t => new
				{
					ticketID = t.TicketID,
					customerID = t.CustomerID,
					customerName = t.CustomerName ?? "",
					employeeID = t.EmployeeID,
					employeeName = t.EmployeeName ?? "",
					subject = t.Subject ?? "",
					description = t.Description ?? "",
					ticketTypeID = t.TicketTypeID,
					ticketTypeName = t.TicketTypeName ?? "",
					statusID = t.StatusID,
					statusName = t.StatusName ?? "",
					priorityID = t.PriorityID,
					priorityName = t.PriorityName ?? "",
					createTime = t.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss")
				}).ToList();

				return Json(list);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetById(int id)
		{
			var t = await _service.GetByIdAsync(id);
			if (t == null) return NotFound();

			var ticket = new
			{
				t.TicketID,
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

		[HttpPost]
		public async Task<IActionResult> CreateTicket([FromBody] CustomerSupportTicketViewModel vm)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var entity = new CustomerSupportTickets
			{
				CustomerID = vm.CustomerID,
				EmployeeID = vm.EmployeeID,
				Subject = vm.Subject,
				TicketTypeID = vm.TicketTypeID,
				Description = vm.Description,
				StatusID = vm.StatusID,
				PriorityID = vm.PriorityID,
				CreateTime = DateTime.Now,
				UpdateTime = DateTime.Now
			};

			await _service.AddAsync(vm); // 保留原 service 邏輯
										 // 取得新增 ID
			var newTicket = await _service.GetAllAsync();
			var createdTicket = newTicket.OrderByDescending(t => t.TicketID).FirstOrDefault();

			return Json(new { success = true, ticketID = createdTicket?.TicketID });
		}


		[HttpPost]
		public async Task<IActionResult> EditTicket([FromBody] CustomerSupportTicketViewModel vm)
		{
			if (!await _service.ExistsAsync(vm.TicketID)) return NotFound();
			await _service.UpdateAsync(vm);
			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> DeleteTicket([FromBody] int id)
		{
			if (!await _service.ExistsAsync(id)) return NotFound();
			await _service.DeleteAsync(id);
			return Json(new { success = true });
		}

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
	}
}
