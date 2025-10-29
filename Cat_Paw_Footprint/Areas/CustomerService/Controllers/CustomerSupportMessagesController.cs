using Cat_Paw_Footprint.Areas.CustomerService.Services; // 匯入客戶服務相關服務
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel; // 匯入 ViewModel
using Cat_Paw_Footprint.Areas.Notification.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Services;
using Microsoft.AspNetCore.Mvc; // 匯入 MVC 控制器
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore; // 匯入 SignalR 即時通訊

namespace Cat_Paw_Footprint.Areas.CustomerService.Controllers
{
	[Area("CustomerService")]
	[Route("CustomerService/[controller]/[action]")]
	[ApiController]
	public class CustomerSupportMessagesController : ControllerBase
	{
		private readonly ICustomerSupportMessagesService _service; // 注入訊息服務
		private readonly IHubContext<TicketChatHub> _hubContext; // SignalR Hub
		private readonly IChatAttachmentService _attachmentService; // 共用附件上傳服務
		private readonly IWebHostEnvironment _env; // 環境物件，用來取 wwwroot 路徑
		private readonly webtravel2Context _db; // 資料庫，用於查找工單與客戶
		private readonly INotificationTriggerService _notifTrigger; // 通知觸發服務


		public CustomerSupportMessagesController(
			ICustomerSupportMessagesService service,
			IHubContext<TicketChatHub> hubContext,
			IChatAttachmentService attachmentService,
			IWebHostEnvironment env,
			webtravel2Context db,
			INotificationTriggerService notifTrigger
		)
		{
			_service = service;
			_hubContext = hubContext;
			_attachmentService = attachmentService;
			_env = env;
			_db = db;
			_notifTrigger = notifTrigger;
		}

		/// <summary>
		/// 取得指定工單的訊息（支援分頁）
		/// GET /CustomerService/CustomerSupportMessages/GetMessages?ticketId={id}&skip={skip}&take={take}
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetMessages(int ticketId, int skip = 0, int take = 30)
		{
			try
			{
				var msgs = await _service.GetByTicketIdAsync(ticketId, skip, take);
				return Ok(msgs);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.ToString());
			}
		}

		/// <summary>
		/// 新增客服訊息（包含文字或附件）
		/// POST /CustomerService/CustomerSupportMessages/PostMessage
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> PostMessage([FromBody] CustomerSupportMessageViewModel vm)
		{
			if (string.IsNullOrWhiteSpace(vm.MessageContent) && string.IsNullOrWhiteSpace(vm.AttachmentURL))
				return BadRequest("訊息內容或附件不可皆為空白");

			try
			{
				// ✅ 儲存訊息
				var result = await _service.AddAsync(vm);
				result.TempId = vm.TempId;

				// ✅ 廣播訊息到 SignalR 群組
				await _hubContext.Clients.Group($"ticket-{vm.TicketID}")
					.SendAsync("ReceiveMessage", result);

				// ✅ 找出該工單的客戶
				var ticket = await _db.CustomerSupportTickets
					.FirstOrDefaultAsync(t => t.TicketID == vm.TicketID);

				if (ticket != null && ticket.CustomerID.HasValue)
				{
					// ✅ 發送通知給該客戶（例如「客服回覆通知」）
					await _notifTrigger.NotifyCustomerServiceReplyAsync(ticket.CustomerID.Value, ticket.TicketID);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// 上傳附件 API（共用 Service）
		/// POST /CustomerService/CustomerSupportMessages/UploadAttachment
		/// </summary>
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
	}
}
