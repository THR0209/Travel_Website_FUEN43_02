using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Authorize(AuthenticationSchemes = "CustomerAuth")]
	[Route("CustomersArea/CustomerService")]
	public class CustomerServiceController : Controller
	{
		private readonly ICustomerSupportTicketsService _ticketService;
		private readonly ICustomerSupportMessagesService _msgService;
		private readonly IHubContext<TicketChatHub> _hubContext;
		private readonly webtravel2Context _context;
		private readonly IChatAttachmentService _attachmentService;
		private readonly IWebHostEnvironment _env;

		public CustomerServiceController(
			ICustomerSupportTicketsService ticketService,
			ICustomerSupportMessagesService msgService,
			IHubContext<TicketChatHub> hubContext,
			webtravel2Context context,
			IChatAttachmentService attachmentService,
			IWebHostEnvironment env
		)
		{
			_ticketService = ticketService;
			_msgService = msgService;
			_hubContext = hubContext;
			_context = context;
			_attachmentService = attachmentService;
			_env = env;
		}

		// ======================= 客服中心頁面 =======================
		[HttpGet("")]
		public IActionResult Index() => View();

		// ======================= 取得工單列表 =======================
		[HttpGet("GetTickets")]
		public async Task<IActionResult> GetTickets()
		{
			var customerIdStr = User.FindFirst("CustomerId")?.Value;
			if (string.IsNullOrEmpty(customerIdStr) || !int.TryParse(customerIdStr, out int customerId))
				return StatusCode(401, new { success = false, message = "請先登入" });

			var tickets = await _context.CustomerSupportTickets
				.Include(t => t.Status)
				.Include(t => t.Employee).ThenInclude(e => e.EmployeeProfile)
				.Where(t => t.CustomerID == customerId)
				.Select(t => new
				{
					ticketID = t.TicketID,
					subject = t.Subject == null ? "(未命名工單)" : t.Subject,
					statusName = t.Status == null ? "未知狀態" : t.Status.StatusDesc,
					employeeName = t.Employee == null
						? "尚未指派"
						: (t.Employee.EmployeeProfile == null
							? "尚未指派"
							: t.Employee.EmployeeProfile.EmployeeName),
					createTime = t.CreateTime.HasValue
						? t.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
						: ""
				})
				.OrderByDescending(t => t.ticketID)
				.ToListAsync();

			return Json(new { success = true, tickets });
		}

		// ======================= 建立新工單 =======================
		[HttpPost("CreateTicket")]
		public async Task<IActionResult> CreateTicket([FromBody] CustomerSupportTicketViewModel vm)
		{
			var customerIdStr = User.FindFirst("CustomerId")?.Value;
			if (string.IsNullOrEmpty(customerIdStr) || !int.TryParse(customerIdStr, out int customerId))
				return StatusCode(401, new { success = false, message = "請先登入" });

			if (vm == null || string.IsNullOrWhiteSpace(vm.Subject) || string.IsNullOrWhiteSpace(vm.Description))
				return BadRequest(new { success = false, message = "請輸入主旨與問題內容" });

			var defaultStatus = await _context.TicketStatus.FirstOrDefaultAsync(s => s.StatusDesc.Contains("待處理"));
			var defaultPriority = await _context.TicketPriority.FirstOrDefaultAsync(p => p.PriorityDesc.Contains("低"));
			var defaultType = await _context.TicketTypes.FirstOrDefaultAsync();

			if (defaultStatus == null || defaultPriority == null)
				return StatusCode(500, new { success = false, message = "找不到預設狀態或優先度。" });

			// 分配客服
			var assignedEmp = await _context.Employees
				.Include(e => e.Role)
				.Where(e => e.Role.RoleName == "CustomerService")
				.OrderBy(e => _context.CustomerSupportTickets
					.Count(t => t.EmployeeID == e.EmployeeID && t.StatusID != 3))
				.FirstOrDefaultAsync();

			if (assignedEmp == null)
				return StatusCode(500, new { success = false, message = "找不到客服人員。" });

			// 產生今日代碼
			var today = DateTime.Now.Date;
			var countToday = await _context.CustomerSupportTickets
				.CountAsync(t => t.CreateTime.HasValue && t.CreateTime.Value.Date == today);
			var newCode = $"CST{today:yyMMdd}{(countToday + 1):D4}";

			var entity = new CustomerSupportTickets
			{
				CustomerID = customerId,
				EmployeeID = assignedEmp.EmployeeID,
				Subject = vm.Subject.Trim(),
				Description = vm.Description.Trim(),
				TicketTypeID = vm.TicketTypeID,
				StatusID = defaultStatus.StatusID,
				PriorityID = defaultPriority.PriorityID,
				CreateTime = DateTime.Now,
				UpdateTime = DateTime.Now,
				TicketCode = newCode
			};

			_context.CustomerSupportTickets.Add(entity);
			await _context.SaveChangesAsync();

			return Json(new
			{
				success = true,
				ticketID = entity.TicketID,
				subject = entity.Subject,
				statusName = defaultStatus.StatusDesc,
				createTime = entity.CreateTime.HasValue
					? entity.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
					: ""
			});
		}

		// ======================= 取得聊天訊息 =======================
		[HttpGet("GetMessages")]
		public async Task<IActionResult> GetMessages(int ticketId)
		{
			var msgs = await _msgService.GetByTicketIdAsync(ticketId, 0, 50);
			return Json(new { success = true, messages = msgs });
		}

		// ======================= 發送訊息 =======================
		[HttpPost("SendMessage")]
		public async Task<IActionResult> SendMessage([FromBody] CustomerSupportMessageViewModel vm)
		{
			if (vm == null || (string.IsNullOrWhiteSpace(vm.MessageContent) && string.IsNullOrWhiteSpace(vm.AttachmentURL)))
				return BadRequest(new { success = false, message = "訊息不可為空" });

			vm.SenderType = "Customer";
			vm.SentBy = User.FindFirst("FullName")?.Value
				?? User.FindFirst("Account")?.Value
				?? "客戶";
			vm.SenderID = int.Parse(User.FindFirst("CustomerId")?.Value ?? "0");
			vm.SentTime = DateTime.Now;

			var msg = await _msgService.AddAsync(vm);

			await _hubContext.Clients.Group($"ticket-{vm.TicketID}")
				.SendAsync("ReceiveMessage", msg);

			return Ok(new { success = true, message = msg });
		}

		// ======================= 上傳附件 =======================
		[HttpPost("UploadAttachment")]
		public async Task<IActionResult> UploadAttachment(IFormFile file)
		{
			try
			{
				var url = await _attachmentService.SaveFileAsync(file);
				return Ok(new { success = true, url });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}

		// ======================= 評價 =======================
		[HttpPost("SubmitFeedback")]
		public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackViewModel vm)
		{
			var customerIdStr = User.FindFirst("CustomerId")?.Value;
			if (string.IsNullOrEmpty(customerIdStr) || !int.TryParse(customerIdStr, out int customerId))
				return StatusCode(401, new { success = false, message = "請先登入" });

			var existing = await _context.CustomerSupportFeedback
				.FirstOrDefaultAsync(f => f.TicketID == vm.TicketID && f.CustomerID == customerId);
			if (existing != null)
				return Ok(new { success = false, message = "已評價過" });

			var ticket = await _context.CustomerSupportTickets.FindAsync(vm.TicketID);
			if (ticket == null)
				return NotFound(new { success = false, message = "找不到工單" });

			var feedback = new CustomerSupportFeedback
			{
				TicketID = vm.TicketID,
				CustomerID = customerId,
				FeedbackRating = vm.Rating,
				FeedbackComment = vm.Comment,
				CreateTime = DateTime.Now
			};

			_context.CustomerSupportFeedback.Add(feedback);

			var completedStatus = await _context.TicketStatus
				.FirstOrDefaultAsync(s => s.StatusDesc.Contains("已完成"));
			if (completedStatus != null)
				ticket.StatusID = completedStatus.StatusID;
			ticket.UpdateTime = DateTime.Now;

			await _context.SaveChangesAsync();
			return Ok(new { success = true });
		}

		// ======================= 取得工單分類 =======================
		[HttpGet("GetTicketTypes")]
		public async Task<IActionResult> GetTicketTypes()
		{
			try
			{
				var types = await _context.TicketTypes
					.Select(t => new
					{
						categoryID = t.TicketTypeID,
						categoryName = t.TicketTypeName
					})
					.ToListAsync();

				return Json(new { success = true, categories = types });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"載入分類失敗: {ex.Message}" });
			}
		}
		// ======================= 檢查是否已評價 =======================
		[HttpGet("GetFeedbackStatus")]
		public async Task<IActionResult> GetFeedbackStatus(int ticketId)
		{
			var customerIdStr = User.FindFirst("CustomerId")?.Value;
			if (string.IsNullOrEmpty(customerIdStr) || !int.TryParse(customerIdStr, out int customerId))
				return StatusCode(401, new { success = false, message = "請先登入" });

			var exists = await _context.CustomerSupportFeedback
				.AnyAsync(f => f.TicketID == ticketId && f.CustomerID == customerId);

			return Ok(new { success = true, hasFeedback = exists });
		}

		// ======================= 取得評價詳細 =======================
		[HttpGet("GetFeedbackDetail")]
		public async Task<IActionResult> GetFeedbackDetail(int ticketId)
		{
			var customerIdStr = User.FindFirst("CustomerId")?.Value;
			if (string.IsNullOrEmpty(customerIdStr) || !int.TryParse(customerIdStr, out int customerId))
				return StatusCode(401, new { success = false, message = "請先登入" });

			var feedback = await _context.CustomerSupportFeedback
				.Where(f => f.TicketID == ticketId && f.CustomerID == customerId)
				.Select(f => new
				{
					rating = f.FeedbackRating,
					comment = f.FeedbackComment,
					createTime = f.CreateTime.HasValue ? f.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""
				})
				.FirstOrDefaultAsync();

			if (feedback == null)
				return Ok(new { success = false, message = "尚未評價" });

			return Ok(new { success = true, feedback });
		}


		// ======================= Feedback ViewModel =======================
		public class FeedbackViewModel
		{
			public int TicketID { get; set; }
			public int Rating { get; set; }
			public string Comment { get; set; }
		}
	}
}
