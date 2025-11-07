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

		// 🟢 取得全台公車路線（逐一載入＋延遲避免被封鎖）
		// 呼叫方式：https://localhost:7132/api/bus/routes
		[HttpGet("routes")]
		public async Task<IActionResult> GetBusRoutes()
		{
			try
			{
				// ✅ 官方 TDX 縣市代碼清單（22 個）
				var cities = new[]
				{
					"Taipei", "NewTaipei", "Taoyuan", "Keelung", "Hsinchu", "HsinchuCounty",
					"MiaoliCounty", "Taichung", "ChanghuaCounty", "NantouCounty",
					"YunlinCounty", "Chiayi", "ChiayiCounty", "Tainan", "Kaohsiung",
					"PingtungCounty", "YilanCounty", "HualienCounty", "TaitungCounty",
					"PenghuCounty", "KinmenCounty", "LienchiangCounty"
				};

				// ✅ 用來儲存所有城市的路線
				var allRoutes = new List<JsonElement>();

				// ✅ 逐一請求（避免 TooManyRequests）
				foreach (var city in cities)
				{
					try
					{
						// 🔹 建立查詢 URL
						var url = $"https://tdx.transportdata.tw/api/basic/v2/Bus/Route/City/{city}?$format=JSON";

						// 🔹 呼叫 TDX API
						var json = await _tdx.GetAsync(url);

						// 🔹 將 JSON 轉成可枚舉資料
						var routes = JsonSerializer.Deserialize<List<JsonElement>>(json);

						// 🔹 如果成功解析則加入集合
						if (routes != null)
						{
							allRoutes.AddRange(routes);
							Console.WriteLine($"✅ 已載入 {city}（{routes.Count} 筆）");
						}
						else
						{
							Console.WriteLine($"⚠️ {city} 無回傳資料（可能該縣市暫無公車路線）");
						}

						// 💤 每次查完一個縣市就延遲 1.5 秒，避免被 TDX 判定過度頻繁
						await Task.Delay(1500);
					}
					catch (Exception ex)
					{
						// ❌ 若個別城市查詢失敗，仍不中斷流程
						Console.WriteLine($"⚠️ 無法載入 {city}：{ex.Message}");

						// ⏳ 延長等待時間讓 TDX API 緩一口氣（防止被短時間封鎖）
						await Task.Delay(2000);
					}
				}

				// ✅ 全部完成
				Console.WriteLine($"✅ 已整合 {allRoutes.Count} 條全台公車路線");

				// ✅ 回傳成功結果給前端
				return Ok(allRoutes);
			}
			catch (Exception ex)
			{
				// ❌ 若整體出現非預期錯誤
				Console.WriteLine($"❌ 無法讀取全台公車路線：{ex.Message}");
				return BadRequest(new { message = "讀取全台公車路線失敗", error = ex.Message });
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
