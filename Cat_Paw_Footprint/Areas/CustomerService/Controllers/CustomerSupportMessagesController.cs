using Cat_Paw_Footprint.Areas.CustomerService.Services; // 匯入客戶服務訊息相關服務介面
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel; // 匯入客戶服務訊息 ViewModel
using Microsoft.AspNetCore.Mvc; // 匯入 MVC 控制器相關功能
using System;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomerService.Controllers
{
	// 設定此 Controller 屬於 CustomerService 區域
	[Area("CustomerService")]
	// 設定路由格式為 CustomerService/[controller]/[action]，controller 會自動替換成 CustomerSupportMessages
	[Route("CustomerService/[controller]/[action]")]
	// 註明這是一個 API Controller，會自動處理模型驗證錯誤並回傳 JSON
	[ApiController]
	public class CustomerSupportMessagesController : ControllerBase
	{
		// 宣告私有欄位，用來儲存注入的客戶服務訊息服務
		private readonly ICustomerSupportMessagesService _service;

		// 透過建構式注入服務
		public CustomerSupportMessagesController(ICustomerSupportMessagesService service)
		{
			_service = service;
		}

		/// <summary>
		/// 取得指定工單的訊息，支援分頁
		/// GET /CustomerService/CustomerSupportMessages/GetMessages?ticketId={ticketId}&skip={skip}&take={take}
		/// </summary>
		/// <param name="ticketId">工單 ID</param>
		/// <param name="skip">跳過的筆數 (預設 0)</param>
		/// <param name="take">取得的筆數 (預設 30)</param>
		/// <returns>訊息列表 (JSON)</returns>
		[HttpGet]
		public async Task<IActionResult> GetMessages(int ticketId, int skip = 0, int take = 30)
		{
			// 依工單 ID 取得訊息列表，並進行分頁 (skip, take)
			var msgs = await _service.GetByTicketIdAsync(ticketId, skip, take);
			return Ok(msgs); // 回傳 200 OK 與訊息 JSON
		}

		/// <summary>
		/// 新增客服訊息
		/// POST /CustomerService/CustomerSupportMessages/PostMessage
		/// </summary>
		/// <param name="vm">訊息資料 (ViewModel)</param>
		/// <returns>新增結果 (JSON)</returns>
		[HttpPost]
		public async Task<IActionResult> PostMessage([FromBody] CustomerSupportMessageViewModel vm)
		{
			// 檢查訊息內容是否為空白，若是則回傳 400 BadRequest
			if (string.IsNullOrWhiteSpace(vm.MessageContent))
				return BadRequest("訊息內容不可空白");

			try
			{
				// 新增訊息
				var result = await _service.AddAsync(vm);
				return Ok(result); // 回傳 200 OK 與新增結果 JSON
			}
			catch (Exception ex)
			{
				// 發生例外時，回傳 500 伺服器錯誤與例外訊息
				return StatusCode(500, ex.ToString());
			}
		}
	}
}