using Cat_Paw_Footprint.Areas.TourGuideArea.Services;
using Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel;
using Cat_Paw_Footprint.Services;
using Cat_Paw_Footprint.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Cat_Paw_Footprint.Areas.TourGuideArea.Controllers
{
	[Area("TourGuideArea")]
	[Route("api/[area]/[controller]/[action]")]
	[ApiController]
	public class TGMessageApiController : ControllerBase
	{
		private readonly ITalkMessageService _msgSvc;
		private readonly ITGAllService _guideSvc;

		public TGMessageApiController(ITalkMessageService msgSvc, ITGAllService guideSvc)
		{
			_msgSvc = msgSvc;
			_guideSvc = guideSvc;
		}

		/// <summary>
		/// 遊客或會員加入群組
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> JoinGroup([FromBody] GroupJoinRequestDto dto)
		{
			if (dto == null || string.IsNullOrWhiteSpace(dto.GroupCode))
				return BadRequest(new { success = false, message = "群組代碼不得為空" });

			var result = await _guideSvc.JoinGroupAsync(dto);
			return Ok(result);
		}

		/// <summary>
		/// 群組發送訊息（導遊 / 客戶 / 遊客）
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> SendMessage([FromBody] GroupMessageRequestDto dto)
		{
			if (dto == null || string.IsNullOrWhiteSpace(dto.GroupCode))
				return BadRequest(new { success = false, message = "群組代碼不得為空" });

			var result = await _msgSvc.SendMessageAsync(dto);
			return Ok(result);
		}

		/// <summary>
		/// 群組上傳照片
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> UploadPhoto([FromBody] GroupPhotoRequestDto dto)
		{
			if (dto == null || string.IsNullOrWhiteSpace(dto.GroupCode) || string.IsNullOrWhiteSpace(dto.PhotoUrl))
				return BadRequest(new { success = false, message = "缺少必要欄位（群組代碼或圖片連結）" });

			var result = await _msgSvc.UploadPhotoAsync(dto);
			return Ok(result);
		}

		/// <summary>
		/// 導遊設定集合地 / 客戶上傳自己位置
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> SetLocation([FromBody] GroupLocationRequestDto dto)
		{
			if (dto == null || dto.GroupId <= 0)
				return BadRequest(new { success = false, message = "群組ID不得為空" });

			var result = await _msgSvc.SetLocationAsync(dto);
			return Ok(result);
		}
		[HttpGet("{groupCode}")]

		public async Task<IActionResult> GetHistory(string groupCode)
		{
			try
			{
				var history = await _msgSvc.GetHistoryAsync(groupCode);
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
				return StatusCode(500, new { message = "伺服器錯誤", error = ex.Message, stack = ex.StackTrace });
			}
		}
	}
}
