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

		// 🟢 取得全台公車路線（分批載入＋延遲避免被封鎖）
		// 呼叫方式：https://localhost:7132/api/bus/routes
		[HttpGet("routes")]
		public async Task<IActionResult> GetBusRoutes()
		{
			try
			{
				// ✅ TDX 支援的縣市代碼（官方格式）
				var cities = new[]
				{
					"Taipei", "NewTaipei", "Taoyuan", "Keelung", "Hsinchu", "HsinchuCounty",
					"MiaoliCounty", "Taichung", "ChanghuaCounty", "NantouCounty",
					"YunlinCounty", "Chiayi", "ChiayiCounty", "Tainan", "Kaohsiung",
					"PingtungCounty", "YilanCounty", "HualienCounty", "TaitungCounty",
					"PenghuCounty", "KinmenCounty", "LienchiangCounty"
				};

				var allRoutes = new List<JsonElement>();

				// ✅ 每批 5 個城市，一批處理完再延遲 1.5 秒
				int batchSize = 5;
				for (int i = 0; i < cities.Length; i += batchSize)
				{
					var batch = cities.Skip(i).Take(batchSize).ToList();

					var tasks = batch.Select(async city =>
					{
						try
						{
							var url = $"https://tdx.transportdata.tw/api/basic/v2/Bus/Route/City/{city}?$format=JSON";
							var json = await _tdx.GetAsync(url);
							var routes = JsonSerializer.Deserialize<List<JsonElement>>(json);

							lock (allRoutes)
							{
								if (routes != null) allRoutes.AddRange(routes);
							}

							Console.WriteLine($"✅ 已載入 {city}（{routes?.Count ?? 0} 筆）");
						}
						catch (Exception ex)
						{
							Console.WriteLine($"⚠️ 無法載入 {city}：{ex.Message}");
						}
					});

					await Task.WhenAll(tasks);

					// 💤 每批之間延遲 1.5 秒（防止 TooManyRequests）
					if (i + batchSize < cities.Length)
					{
						Console.WriteLine("⏳ 等待 1.5 秒再繼續下一批...");
						await Task.Delay(1500);
					}
				}

				Console.WriteLine($"✅ 已整合 {allRoutes.Count} 條全台公車路線");
				return Ok(allRoutes);
			}
			catch (Exception ex)
			{
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
