using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;



namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Authorize(AuthenticationSchemes = "CustomerAuth")]
	[Route("CustomersArea/[controller]/[action]")]
	public class CustomerServiceController : Controller
	{
		private readonly ICustomerSupportTicketsService _ticketService;
		private readonly ICustomerSupportMessagesService _msgService;
		private readonly IHubContext<TicketChatHub> _hubContext;
		private readonly webtravel2Context _context;
		private readonly IChatAttachmentService _attachmentService;
		private readonly IWebHostEnvironment _env;
		private readonly INotificationTriggerService _notifTrigger;

		public CustomerServiceController(
			ICustomerSupportTicketsService ticketService,
			ICustomerSupportMessagesService msgService,
			IHubContext<TicketChatHub> hubContext,
			webtravel2Context context,
			IChatAttachmentService attachmentService,
			IWebHostEnvironment env,
			INotificationTriggerService notifTrigger
		)
		{
			_ticketService = ticketService;
			_msgService = msgService;
			_hubContext = hubContext;
			_context = context;
			_attachmentService = attachmentService;
			_env = env;
			_notifTrigger = notifTrigger;
		}

		// ======================= 客服中心頁面 =======================

		/// <summary>
		/// 客服中心主頁面
		/// GET: /CustomersArea/CustomerService/Index
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Index() => View();

		// ======================= 取得工單列表 =======================

		/// <summary>
		/// 取得目前客戶的所有工單
		/// GET: /CustomersArea/CustomerService/GetTickets
		/// </summary>
		/// <returns></returns>
		[HttpGet]
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

		/// <summary>
		/// 建立新客服工單（前台客戶端）
		/// </summary>
		/// <param name="vm"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> CreateTicket([FromBody] CustomerSupportTicketViewModel vm)
		{
			var customerIdStr = User.FindFirst("CustomerId")?.Value;
			if (string.IsNullOrEmpty(customerIdStr) || !int.TryParse(customerIdStr, out int customerId))
				return StatusCode(401, new { success = false, message = "請先登入" });

			if (vm == null || string.IsNullOrWhiteSpace(vm.Subject) || string.IsNullOrWhiteSpace(vm.Description))
				return BadRequest(new { success = false, message = "請輸入主旨與問題內容" });

			// ✅ 檢查分類是否存在
			var ticketType = await _context.TicketTypes
				.FirstOrDefaultAsync(t => t.TicketTypeID == vm.TicketTypeID);
			if (ticketType == null)
				return BadRequest(new { success = false, message = "請選擇有效的分類" });

			var defaultStatus = await _context.TicketStatus
				.FirstOrDefaultAsync(s => s.StatusDesc.Contains("待處理"));
			var defaultPriority = await _context.TicketPriority
				.FirstOrDefaultAsync(p => p.PriorityDesc.Contains("低"));

			if (defaultStatus == null || defaultPriority == null)
				return StatusCode(500, new { success = false, message = "找不到預設狀態或優先度。" });

			// ✅ 分配客服（找最少工單的員工）
			var assignedEmp = await _context.Employees
				.Include(e => e.Role)
				.Where(e => e.Role.RoleName == "CustomerService")
				.OrderBy(e => _context.CustomerSupportTickets
					.Count(t => t.EmployeeID == e.EmployeeID && t.StatusID != 3))
				.FirstOrDefaultAsync();

			if (assignedEmp == null)
				return StatusCode(500, new { success = false, message = "找不到客服人員。" });

			// 產生工單代碼
			var today = DateTime.Now.Date;
			var countToday = await _context.CustomerSupportTickets
				.CountAsync(t => t.CreateTime.HasValue && t.CreateTime.Value.Date == today);
			var newCode = $"CST{today:yyMMdd}{(countToday + 1):D4}";

			try
			{
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
					createTime = entity.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss")
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = $"建立工單時發生錯誤：{ex.Message}" });
			}
		}

		// ======================= 取得聊天訊息 =======================

		/// <summary>
		/// 取得指定工單的聊天訊息（前台客戶端）
		/// GET: /CustomersArea/CustomerService/GetMessages?ticketId={id}
		/// </summary>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetMessages(int ticketId)
		{
			var msgs = await _msgService.GetByTicketIdAsync(ticketId, 0, 50);
			return Json(new { success = true, messages = msgs });
		}

		// ======================= 發送訊息 =======================

		/// <summary>
		/// 發送客服訊息（前台客戶端）
		/// POST: /CustomersArea/CustomerService/SendMessage
		/// </summary>
		/// <param name="vm"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> SendMessage([FromBody] CustomerSupportMessageViewModel vm)
		{
			var customerIdStr = User.FindFirst("CustomerId")?.Value;
			if (string.IsNullOrEmpty(customerIdStr) || !int.TryParse(customerIdStr, out int customerId))
				return StatusCode(401, new { success = false, message = "請先登入" });

			if (vm == null || (string.IsNullOrWhiteSpace(vm.MessageContent) && string.IsNullOrWhiteSpace(vm.AttachmentURL)))
				return BadRequest(new { success = false, message = "訊息不可為空" });

			vm.SenderType = "Customer";
			vm.SentBy = User.FindFirst("FullName")?.Value
				?? User.FindFirst("Account")?.Value
				?? "客戶";
			vm.SenderID = customerId;
			vm.SentTime = DateTime.Now;

			var msg = await _msgService.AddAsync(vm);
			//聊天室即時傳送
			await _hubContext.Clients.Group($"ticket-{vm.TicketID}").SendAsync("ReceiveMessage", msg);

			return Ok(new { success = true, message = msg });
		}

		// ======================= 單檔上傳附件 =======================

		/// <summary>
		/// 上傳聊天圖片（共用 Service）
		/// POST: /CustomersArea/CustomerService/UploadAttachment
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> UploadAttachment(IFormFile file)
		{
			try
			{
				if (file == null)
					return BadRequest(new { success = false, message = "未選擇任何圖片。" });

				// 格式白名單
				string[] allowedTypes = { "image/png", "image/jpeg", "image/gif", "image/webp", "application/pdf" };
				if (!allowedTypes.Contains(file.ContentType))
					return BadRequest(new { success = false, message = "僅支援圖片或 PDF 檔案。" });

				// 檔案大小上限
				if (file.Length > 10 * 1024 * 1024)
					return BadRequest(new { success = false, message = "檔案大小超過 10MB。" });

				var url = await _attachmentService.SaveFileAsync(file);
				return Ok(new { success = true, url });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}



		// ======================= 多檔上傳附件 =======================

		/// <summary>
		/// 多檔上傳附件 API（共用 Service）
		/// POST: /CustomersArea/CustomerService/UploadMultipleAttachments
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> UploadMultipleAttachments(List<IFormFile> files)
		{
			try
			{
				// 1️ 檢查是否有上傳檔案
				if (files == null || files.Count == 0)
					return BadRequest(new { success = false, message = "未選擇任何檔案。" });

				// 2️ 限制檔案數量（例如最多 5 個）
				if (files.Count > 5)
					return BadRequest(new { success = false, message = "一次最多只能上傳 5 個檔案。" });

				// 3️ 檔案格式限制（允許圖片 / PDF）
				string[] allowedTypes = { "image/png", "image/jpeg", "image/gif", "image/webp", "application/pdf" };
				var invalidFiles = files.Where(f => !allowedTypes.Contains(f.ContentType)).ToList();
				if (invalidFiles.Any())
					return BadRequest(new
					{
						success = false,
						message = "僅支援圖片或 PDF 格式。",
						files = invalidFiles.Select(f => f.FileName)
					});

				// 4️ 檔案大小限制（例如：每檔 10MB 以下）
				long maxSize = 10 * 1024 * 1024; // 10MB
				var oversizeFiles = files.Where(f => f.Length > maxSize).ToList();
				if (oversizeFiles.Any())
					return BadRequest(new
					{
						success = false,
						message = "部分檔案超過 10MB，請重新上傳。",
						files = oversizeFiles.Select(f => f.FileName)
					});

				// 5️ 上傳
				var uploadTasks = files.Select(f => _attachmentService.SaveFileAsync(f));
				var urls = await Task.WhenAll(uploadTasks);

				return Ok(new { success = true, urls });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}

		// ======================= 評價 =======================

		/// <summary>
		///	評價客服服務
		///	POST: /CustomersArea/CustomerService/SubmitFeedback
		/// </summary>
		/// <param name="vm"></param>
		/// <returns></returns>
		[HttpPost]
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

		/// <summary>
		/// 取得工單分類列表
		/// GET: /CustomersArea/CustomerService/GetTicketTypes
		/// </summary>
		/// <returns></returns>
		[HttpGet]
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

		/// <summary>
		/// 檢查指定工單是否已評價
		/// GET: /CustomersArea/CustomerService/GetFeedbackStatus?ticketId={id}
		/// </summary>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		[HttpGet]
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
		/// <summary>
		/// 取得指定工單的評價詳細
		/// GET: /CustomersArea/CustomerService/GetFeedbackDetail?ticketId={id}
		/// </summary>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		[HttpGet]
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
