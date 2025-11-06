using Cat_Paw_Footprint.Areas.CustomersArea.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Route("api/[controller]")]
	[ApiController]
	public class BusController : ControllerBase
	{
		private readonly TdxService _tdxService;

		public BusController(TdxService tdxService)
		{
			_tdxService = tdxService;
		}

		// 🚍 即時到站時間查詢
		[HttpGet("eta")]
		public async Task<IActionResult> GetBusETA(string city, string route)
		{
			if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(route))
				return BadRequest("請輸入城市與公車路線");

			try
			{
				var url = $"https://tdx.transportdata.tw/api/basic/v2/Bus/EstimatedTimeOfArrival/City/{city}?$filter=RouteName/Zh_tw eq '{route}'&$format=JSON";
				var json = await _tdxService.GetAsync(url);

				Console.WriteLine($"✅ 查詢成功：{city}-{route}, 回傳長度：{json?.Length ?? 0}");

				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ 公車查詢錯誤：{ex.Message}");
				return StatusCode(500, new { error = ex.Message });
			}
		}

	}
}
