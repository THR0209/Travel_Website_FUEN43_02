using Cat_Paw_Footprint.Areas.CustomersArea.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Route("api/bus")]
	public class BusController : Controller
	{
		private readonly TdxService _tdx;   // ✅ 注入你的服務

		public BusController(TdxService tdx)
		{
			_tdx = tdx;
		}

		// 🟢 取得全台公車路線
		// 呼叫方式：https://localhost:7132/api/bus/routes
		[HttpGet("routes")]
		public async Task<IActionResult> GetBusRoutes()
		{
			try
			{
				// ✅ 使用你的 TDX Service 取資料（自動帶 token）
				var url = "https://tdx.transportdata.tw/api/basic/v2/Bus/Route/City/Taipei?$format=JSON";
				var json = await _tdx.GetAsync(url);

				var list = JsonSerializer.Deserialize<List<object>>(json);
				if (list == null)
					return BadRequest(new { message = "TDX 資料格式錯誤" });

				return Ok(list);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "讀取公車路線失敗", error = ex.Message });
			}
		}

		// 🟢 查詢即時到站資訊 (ETA)
		[HttpGet("eta")]
		public async Task<IActionResult> GetBusETA(string city, string route)
		{
			try
			{
				var url = $"https://tdx.transportdata.tw/api/basic/v2/Bus/EstimatedTimeOfArrival/City/{city}/{route}?$format=JSON";
				var json = await _tdx.GetAsync(url);
				var list = JsonSerializer.Deserialize<List<object>>(json);
				return Ok(list);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "讀取 ETA 資料失敗", error = ex.Message });
			}
		}

		// 🟢 查詢附近站牌
		[HttpGet("nearby")]
		public async Task<IActionResult> GetNearbyStops(double lat = 25.0478, double lng = 121.5319)
		{
			try
			{
				var url = $"https://tdx.transportdata.tw/api/basic/v2/Bus/Station/NearBy?$top=30&$spatialFilter=nearby({lat},{lng},1000)&$format=JSON";
				var json = await _tdx.GetAsync(url);
				var list = JsonSerializer.Deserialize<List<object>>(json);
				return Ok(list);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "讀取附近站牌失敗", error = ex.Message });
			}
		}
	}
}
