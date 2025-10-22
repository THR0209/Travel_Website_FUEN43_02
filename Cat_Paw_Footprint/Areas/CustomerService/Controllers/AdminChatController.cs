using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.AdminArea.Controllers
{
	[Area("AdminArea")]
	[Route("AdminArea/[controller]/[action]")]
	public class AdminChatController : Controller
	{
		private readonly ICustomerSupportTicketsService _ticketService;
		private readonly ICustomerSupportMessagesService _msgService;
		private readonly IHubContext<TicketChatHub> _hubContext;
		private readonly webtravel2Context _context;

		public AdminChatController(
			ICustomerSupportTicketsService ticketService,
			ICustomerSupportMessagesService msgService,
			IHubContext<TicketChatHub> hubContext,
			webtravel2Context context)
		{
			_ticketService = ticketService;
			_msgService = msgService;
			_hubContext = hubContext;
			_context = context;
		}

		/// <summary>
		/// 取得所有工單清單（可用於後台客服列表）
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetAllTickets()
		{
			var tickets = await _ticketService.GetAllAsync();
			return Json(new { success = true, data = tickets });
		}

		/// <summary>
		/// 依工單 ID 取得詳細資料（含客戶資訊）
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetTicketDetails(int ticketId)
		{
			var ticket = await _ticketService.GetByIdAsync(ticketId);
			if (ticket == null)
				return NotFound("找不到此工單");

			var customer = await _context.Customers
				.Include(c => c.CustomerProfile)
				.FirstOrDefaultAsync(c => c.CustomerID == ticket.CustomerID);

			return Json(new
			{
				success = true,
				ticket,
				customerName = customer?.CustomerProfile?.CustomerName ?? customer?.FullName ?? "未知客戶"
			});
		}


		/// <summary>
		/// 取得指定工單的聊天訊息（後台客服端）
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetMessages(int ticketId)
		{
			var msgs = await _msgService.GetByTicketIdAsync(ticketId, 0, 100);
			return Json(new { success = true, messages = msgs });
		}

		/// <summary>
		/// 後台客服回覆訊息
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> SendReply([FromBody] CustomerSupportMessageViewModel vm)
		{
			if (vm == null || string.IsNullOrWhiteSpace(vm.MessageContent))
				return BadRequest("訊息不可為空");

			// 標記為客服端身分
			vm.SenderType = "Admin";
			vm.SentBy = "客服人員"; // 可改為實際登入員工名稱
			vm.SentTime = DateTime.Now;

			var msg = await _msgService.AddAsync(vm);

			// 廣播給該聊天室（前台客戶也會收到）
			await _hubContext.Clients.Group($"ticket-{vm.TicketID}")
				.SendAsync("ReceiveMessage", msg);

			return Ok(new { success = true, message = msg });
		}

		/// <summary>
		/// 更新工單狀態（例如：處理中、已完成）
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> UpdateTicketStatus(int ticketId, int statusId)
		{
			var ticket = await _context.CustomerSupportTickets.FindAsync(ticketId);
			if (ticket == null)
				return NotFound("找不到此工單");

			ticket.StatusID = statusId;
			ticket.UpdateTime = DateTime.Now;

			await _context.SaveChangesAsync();
			return Ok(new { success = true });
		}

		/// <summary>
		/// 取得工單的客戶回饋（評價）
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetFeedback(int ticketId)
		{
			var feedback = await _context.CustomerSupportFeedback
				.FirstOrDefaultAsync(f => f.TicketID == ticketId);

			if (feedback == null)
				return Json(new { success = false, message = "尚未有回饋" });

			return Json(new { success = true, data = feedback });
		}
	}
}
