using Cat_Paw_Footprint.Areas.CustomerService.Services; // 匯入客戶服務相關服務
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel; // 匯入 ViewModel
using Microsoft.AspNetCore.Mvc; // 匯入 MVC 控制器
using Microsoft.AspNetCore.SignalR; // 匯入 SignalR 即時通訊
using Microsoft.AspNetCore.Hosting; // IWebHostEnvironment
using Microsoft.AspNetCore.Http; // IFormFile
using System;
using System.IO;
using System.Threading.Tasks;

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

		public CustomerSupportMessagesController(
			ICustomerSupportMessagesService service,
			IHubContext<TicketChatHub> hubContext,
			IChatAttachmentService attachmentService,
			IWebHostEnvironment env 
		)
		{
			_service = service;
			_hubContext = hubContext;
			_attachmentService = attachmentService;
			_env = env;
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
				var result = await _service.AddAsync(vm);
				result.TempId = vm.TempId;

				// SignalR 廣播給群組 (ticket-<id>)
				await _hubContext.Clients.Group($"ticket-{vm.TicketID}")
					.SendAsync("ReceiveMessage", result);

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.ToString());
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
