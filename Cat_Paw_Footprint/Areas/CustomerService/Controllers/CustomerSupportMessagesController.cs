using Cat_Paw_Footprint.Areas.Helper;
using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Cat_Paw_Footprint.Areas.Notification.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Cat_Paw_Footprint.Areas.CustomerService.Controllers
{
	[Area("CustomerService")]
	[Route("CustomerService/[controller]/[action]")]
	[ApiController]
	public class CustomerSupportMessagesController : ControllerBase
	{
		private readonly ICustomerSupportMessagesService _service;
		private readonly IHubContext<TicketChatHub> _hubContext;
		private readonly webtravel2Context _db;
		private readonly INotificationTriggerService _notifTrigger;

		public CustomerSupportMessagesController(
			ICustomerSupportMessagesService service,
			IHubContext<TicketChatHub> hubContext,
			webtravel2Context db,
			INotificationTriggerService notifTrigger)
		{
			_service = service;
			_hubContext = hubContext;
			_db = db;
			_notifTrigger = notifTrigger;
		}

		/// <summary> 
		/// 取得指定工單的訊息（支援分頁） 
		/// /// GET /CustomerService/CustomerSupportMessages/GetMessages?ticketId={id}&skip={skip}&take={take} 
		/// /// </summary> 
		[HttpGet]
		public async Task<IActionResult> GetMessages(int ticketId, int skip = 0, int take = 30)
		{
			try
			{ 
				var msgs = await _service.GetByTicketIdAsync(ticketId, skip, take);
				return Ok(msgs); 
			}
			catch (Exception ex) { 
				return StatusCode(500, ex.ToString()); 
			}
		}

		/// <summary>
		/// 新增客服訊息（文字或附件）
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> PostMessage([FromBody] CustomerSupportMessageViewModel vm)
		{
			try
			{
				if (vm == null)
					return BadRequest("未收到任何資料。");

				// ✅ 若 attachmentURL 是 JSON 陣列，轉成字串
				if (vm.AttachmentURL != null && vm.AttachmentURL.StartsWith("["))
				{
					var arr = JArray.Parse(vm.AttachmentURL);
					vm.AttachmentURL = string.Join(",", arr.Select(x => x.ToString()));
				}

				if (string.IsNullOrWhiteSpace(vm.MessageContent) && string.IsNullOrWhiteSpace(vm.AttachmentURL))
					return BadRequest("訊息內容或附件不可皆為空白");

				var result = await _service.AddAsync(vm);
				result.TempId = vm.TempId;

				await _hubContext.Clients.Group($"ticket-{vm.TicketID}")
					.SendAsync("ReceiveMessage", result);

				// 發送通知給客戶（如果有 CustomerID）
				var ticket = await _db.CustomerSupportTickets.FirstOrDefaultAsync(t => t.TicketID == vm.TicketID);
				if (ticket?.CustomerID != null)

					await _notifTrigger.NotifyCustomerServiceReplyAsync(ticket.CustomerID.Value, ticket.TicketID);

				return Ok(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ PostMessage 發生錯誤：{ex}");
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// 上傳單一附件（使用 ImgBB）
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> UploadAttachment(IFormFile file)
		{
			try
			{
				if (file == null)
					return BadRequest(new { success = false, message = "未選擇檔案。" });

				var url = await ImgBBHelper.UploadSingleImageAsync(file);
				return Ok(new { success = true, url });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ UploadAttachment 錯誤：{ex}");
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// 上傳多個附件（圖片）→ ImgBB
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> UploadMultipleAttachments(List<IFormFile> files)
		{
			try
			{
				if (files == null || files.Count == 0)
					return BadRequest(new { success = false, message = "未選擇任何檔案。" });

				var allowed = new[] { "image/png", "image/jpeg", "image/jpg", "image/gif", "image/webp" };
				if (files.Any(f => !allowed.Contains(f.ContentType)))
					return BadRequest(new { success = false, message = "僅支援圖片格式（PNG、JPG、GIF、WEBP）" });

				if (files.Any(f => f.Length > 25 * 1024 * 1024))
					return BadRequest(new { success = false, message = "檔案大小不可超過 25MB。" });

				var urls = await ImgBBHelper.UploadImagesAsync(files);
				return Ok(new { success = true, urls });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ UploadMultipleAttachments 錯誤：{ex}");
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}
	}
}
