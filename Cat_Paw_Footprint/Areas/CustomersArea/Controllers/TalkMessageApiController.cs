using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Services;
using Cat_Paw_Footprint.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class TalkMessageApiController: ControllerBase
	{
		private readonly ITalkMessageService _service;
		public TalkMessageApiController(ITalkMessageService service)
		{
			_service = service;
		}
		[HttpPost]
		public async Task<IActionResult> JoinAsCustomer([FromBody] CusJoinDto dto)//會員加入群組
		{
			// 1️⃣ 從登入 Cookie 中讀取 Claims
			var idClaim = User.FindFirst("CustomerId");
			var nameClaim = User.FindFirst("FullName");

			if (idClaim == null)
				return Unauthorized(new { message = "尚未登入" });

			int customerId = int.Parse(idClaim.Value);
			string? customerName = nameClaim?.Value;

			// 2️⃣ 呼叫 Service 寫入資料庫
			var result = await _service.JoinGroupbyCustomerAsync(dto.GroupCode, customerId, customerName);
			if (result == "成功加入群組")
			{
				Console.WriteLine($"✅ 會員 {customerId} 成功加入群組 {dto.GroupCode}");
			}
			else
			{
				Console.WriteLine($"❌ 會員 {customerId} 加入群組 {dto.GroupCode} 失敗: {result}");
			}
			return Ok(new { message = result });
		}

		
		[HttpPost]
		public async Task<IActionResult> JoinAsGuest([FromBody] GuestJoinDto dto)// 訪客加入群組
		{
			if (dto == null || string.IsNullOrWhiteSpace(dto.GroupCode) || string.IsNullOrWhiteSpace(dto.DeviceId))
				return BadRequest(new { message = "資料不完整" });

			var result = await _service.JoinGuestbyDeviceAsync(dto.GroupCode, dto.TemporaryName, dto.DeviceId);
			if (result == "成功加入群組")
			{
				Console.WriteLine($"✅ 訪客 {dto.DeviceId} 成功加入群組 {dto.GroupCode}");

			}
			else
			{
				Console.WriteLine($"❌ 訪客 {dto.DeviceId} 加入群組 {dto.GroupCode} 失敗: {result}");
			}
			return Ok(new { message = result });
		}

		[HttpPost]
		public async Task<IActionResult> Send([FromBody] GroupMessageRequestDto dto)// 發送群組訊息
		{
			// 自動判斷身分
			if (string.IsNullOrEmpty(dto.SenderType))
			{
				dto.SenderType = User.Identity?.IsAuthenticated == true ? "Customer" : "Guest";
			}

			var result = await _service.SendMessageAsync(dto);
			return Ok(result);
		}
		[HttpPost]
		public async Task<IActionResult> UploadPhoto([FromBody] GroupPhotoRequestDto dto)// 上傳群組照片
		{
			var result = await _service.UploadPhotoAsync(dto);
			return Ok(result);
		}
		[HttpGet("{groupCode}")]
		public async Task<IActionResult> GetHistory(string groupCode)
		{
			try
			{
				var history = await _service.GetHistoryAsync(groupCode);
				if (history == null || !history.Any())
					return Ok(new List<object>());

				var safeList = history.Select(m => new {
					m.MessageId,
					m.SenderType,
					m.Content,
					SendTime = m.SendTime.ToString("yyyy-MM-dd HH:mm:ss")
				});

				return Ok(safeList);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ 取得歷史訊息失敗: {ex}");
				return StatusCode(500, new { message = "伺服器錯誤", error = ex.Message });
			}
		}
		[HttpPost]
		public async Task<IActionResult> LeaveGroup([FromBody] string groupCode)// 離開群組
		{
			// 這裡通常不需要做什麼，因為 SignalR Hub 會處理離開群組的邏輯
			// 但可以在這裡做一些驗證或紀錄
			Console.WriteLine($"✅ 使用者請求離開群組 {groupCode}");
			return Ok(new { success = true, message = $"已請求離開群組 {groupCode}" });
		}
	}
}
