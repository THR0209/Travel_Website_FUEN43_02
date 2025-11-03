using Cat_Paw_Footprint.Areas.TourGuideArea.Services;
using Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel;
using Cat_Paw_Footprint.ViewModel;
using Microsoft.AspNetCore.Mvc;


namespace Cat_Paw_Footprint.Areas.TourGuideArea.Controllers
{
	[Area("TourGuideArea")]
	[Route("api/[area]/[controller]/[action]")]
	[ApiController]
	public class TGGroupApiController : ControllerBase
	{
		private readonly ITGAllService _svc;

		public TGGroupApiController(ITGAllService svc)
		{
			_svc = svc;
		}
		/// <summary>
		/// 導遊建立新群組
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> CreateGroup([FromBody] GroupCreateRequestDto dto)
		{
			if (dto == null || dto.GuideId <= 0 || string.IsNullOrWhiteSpace(dto.GroupName))
				return BadRequest(new { success = false, message = "導遊ID與群組名稱不得為空" });

			var result = await _svc.CreateGroupAsync(dto);
			if (result == null || result.Success == false)
				return BadRequest(new { success = false, message = "群組建立失敗" });

			return Ok(result);
		}

		/// <summary>
		/// 根據導遊ID取得群組清單
		/// </summary>
		[HttpGet("{guideId}")]
		public async Task<IActionResult> GetGroupsByGuideId(int guideId)
		{
			if (guideId <= 0)
				return BadRequest(new { success = false, message = "無效的導遊ID" });

			var result = await _svc.GetGroupsByGuideIdAsync(guideId);
			return Ok(result);
		}

		/// <summary>
		/// 根據群組ID取得群組詳細資料（含訊息）
		/// </summary>
		[HttpGet("{groupId}")]
		public async Task<IActionResult> GetGroupDetail(int groupId)
		{
			if (groupId <= 0)
				return BadRequest(new { success = false, message = "無效的群組ID" });

			var result = await _svc.GetGroupDetailByGroupIdAsync(groupId);
			if (result == null)
				return NotFound(new { success = false, message = "查無此群組" });

			return Ok(result);
		}

	}
}
