using Cat_Paw_Footprint.Areas.TourGuideArea.Services;
using Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel;
using Microsoft.AspNetCore.Mvc;
namespace Cat_Paw_Footprint.Areas.TourGuideArea.Controllers
{
	[Area("TourGuideArea")]
	[Route("api/[area]/[controller]/[action]")]
	[ApiController]
	public class TGLoginApiController : ControllerBase
	{
		private readonly ITGAllService _service;

		public TGLoginApiController(ITGAllService service)
		{
			_service = service;
		}

		/// <summary>
		/// 導遊登入驗證
		/// </summary>
		/// <param name="dto">包含帳號與密碼</param>
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] TGLoginRequestDto dto)
		{
			if (dto == null || string.IsNullOrWhiteSpace(dto.Account) || string.IsNullOrWhiteSpace(dto.Password))
				return BadRequest(new { success = false, message = "帳號或密碼不得為空" });

			var result = await _service.GetGuideByAccountAsync(dto);
			if (result == null)
				return NotFound(new { success = false, message = "查無此帳號" });


			return Ok(result);
		}
	}
}
