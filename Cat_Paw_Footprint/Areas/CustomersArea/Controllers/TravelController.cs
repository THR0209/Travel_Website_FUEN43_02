using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
//using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[AllowAnonymous]
	public class TravelController : Controller
	{
		private readonly webtravel2Context _context;    // 資料庫 Context
		private readonly IConfiguration _config;    // appsettings.json 設定檔

		/* 建構子：注入資料庫 Context */
		public TravelController(webtravel2Context context, IConfiguration config)
		{
			// 把設定注入
			_context = context; // 取得資料庫 Context
			_config = config;   // 取得 appsettings.json 設定  
		}

		/* 主頁面 View (顯示 Vue 畫面) */
		public IActionResult Index()
		{
			// 從 secrets.json 取得金鑰
			ViewBag.GoogleMapKey = _config["GoogleMaps:ApiKey"];
			return View();
		}

		/* 取得目前登入使用者資訊 */
		[HttpGet("/api/currentUser")]
		[Authorize(AuthenticationSchemes = "CustomerAuth")]
		public IActionResult GetCurrentUser()
		{
			if (!User.Identity.IsAuthenticated)
				return Unauthorized();

			var userInfo = new
			{
				CustomerId = User.FindFirst("CustomerId")?.Value,
				Account = User.FindFirst("Account")?.Value,
				FullName = User.FindFirst("FullName")?.Value,
			};

			return Json(userInfo);
		}

		/* 取得【住宿資料】（給 Vue3 呼叫用） */
		[HttpGet("/api/hotels")]
		public async Task<IActionResult> GetHotels()
		{
			var data = await _context.Hotels
				.Include(h => h.Region)
				.Include(h => h.District)
				.Include(h => h.HotelPics)
				.Where(h => h.IsActive == true)
				.Select(h => new HotelsViewModel
				{
					HotelID = h.HotelID,
					HotelName = h.HotelName,
					HotelAddr = h.HotelAddr,
					HotelLat = h.HotelLat,
					HotelLng = h.HotelLng,
					HotelDesc = h.HotelDesc,
					Rating = h.Rating,
					Views = h.Views,
					RegionID = h.RegionID,
					RegionName = h.Region.RegionName,
					DistrictID = h.DistrictID,
					DistrictName = h.District.DistrictName,
					// 此行必須要有 HotelPics.PictureUrl 屬性才會成功傳出多張圖。
					PictureUrl = h.HotelPics.Select(p => p.PictureUrl).ToList()
				}).ToListAsync();

			return Json(data);  // 回傳 JSON 格式資料
		}

		/* 取得【景點資料】（給 Vue3 呼叫用） */
		[HttpGet("/api/locations")]
		public async Task<IActionResult> GetLocations()
		{
			var data = await _context.Locations
				.Include(l => l.Region)
				.Include(l => l.District)
				.Include(l => l.LocationPics)
				.Where(l => l.IsActive == true)
				.Select(l => new LocationsViewModel
				{
					LocationID = l.LocationID,
					LocationName = l.LocationName,
					LocationAddr = l.LocationAddr,
					LocationLat = l.LocationLat,
					LocationLng = l.LocationLng,
					LocationDesc = l.LocationDesc,
					Rating = l.Rating,
					Views = l.Views,
					RegionID = l.RegionID,
					RegionName = l.Region.RegionName,
					DistrictID = l.DistrictID,
					DistrictName = l.District.DistrictName,
					// 此行必須要 LocationPics.PictureUrl 屬性才會成功傳出多張圖。
					PictureUrl = l.LocationPics.Select(p => p.PictureUrl).ToList()
				}).ToListAsync();

			return Json(data);  // 回傳 JSON 格式資料
		}

		/* 取得【美食資料】（給 Vue3 呼叫用） */
		[HttpGet("/api/restaurants")]
		public async Task<IActionResult> GetRestaurants()
		{
			var data = await _context.Restaurants
				.Include(r => r.Region)
				.Include(r => r.District)
				.Include(r => r.RestaurantPics)
				.Where(r => r.IsActive == true)
				.Select(r => new RestaurantsViewModel
				{
					RestaurantID = r.RestaurantID,
					RestaurantName = r.RestaurantName,
					RestaurantAddr = r.RestaurantAddr,
					RestaurantLat = r.RestaurantLat,
					RestaurantLng = r.RestaurantLng,
					RestaurantDesc = r.RestaurantDesc,
					Rating = r.Rating,
					Views = r.Views,
					RegionID = r.RegionID,
					RegionName = r.Region.RegionName,
					DistrictID = r.DistrictID,
					DistrictName = r.District.DistrictName,
					// 此行必須要 RestaurantPics.PictureUrl 屬性才會成功傳出多張圖。
					PictureUrl = r.RestaurantPics.Select(p => p.PictureUrl).ToList()
				}).ToListAsync();

			return Json(data);  // 回傳 JSON 格式資料
		}

		/* 儲存行程 (接收前端 JSON) */
		[HttpPost("/api/trips/save")]
		[Authorize(AuthenticationSchemes = "CustomerAuth")]
		public async Task<IActionResult> SaveTrip([FromBody] JsonElement json)
		{

			TripProjectViewModel? data;

			try
			{
				// 反序列化 JSON
				data = JsonSerializer.Deserialize<TripProjectViewModel>(
					json.GetRawText(),
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true  // 大小寫不敏感
					});
			}
			catch (Exception ex)
			{
				return BadRequest($"❌ 無法解析 JSON 資料: {ex.Message}");
			}

			// 更清楚的錯誤提示（逐層檢查）
			if (data == null)
				return BadRequest("❌ 未收到資料");
			if (data.Details == null)
				return BadRequest("❌ 明細資料為空");
			if (!data.Details.Any())
				return BadRequest("❌ 行程明細無內容");

			try
			{
				// 建立主檔 CustomerTripProjects
				var project = new CustomerTripProjects
				{
					CustomerID = data.CustomerID,
					ProjectName = data.ProjectName ?? "未命名行程",
					TotalDays = data.TotalDays ?? 1,
					CreateTime = DateTime.Now,
					UpdateTime = DateTime.Now
				};

				_context.CustomerTripProjects.Add(project);
				await _context.SaveChangesAsync(); // 儲存後才能取得 ProjectID

				// 建立多筆明細 TripProjectDetails
				var details = data.Details.Select(d =>
				{
					// 嘗試將 "12:30" 轉成 TimeSpan(12,30,0)
					TimeSpan parsedTime = TimeSpan.Zero;
					if (!string.IsNullOrWhiteSpace(d.StartTime))
						TimeSpan.TryParse(d.StartTime, out parsedTime);

					return new TripProjectDetails
					{
						ProjectID = project.ProjectID,
						TripDate = d.TripDate,
						TripSequence = d.TripSequence ?? 0,
						StartTime = parsedTime,                // 安全轉換後使用
						StayMinute = d.StayMinute ?? 0,
						TripType = d.TripType,

						// 三種行程類別 ID 對應
						HotelID = d.HotelID,
						LocationID = d.LocationID,
						RestaurantID = d.RestaurantID,

						Notes = d.Notes
					};
				}).ToList();

				_context.TripProjectDetails.AddRange(details);
				await _context.SaveChangesAsync();

				// 回傳結果給前端
				return Ok("行程已成功儲存！");
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
					return StatusCode(500,"Inner Exception: " + ex.InnerException.Message);

				// 若有錯誤，回傳 500 錯誤碼與訊息
				return StatusCode(500, $"儲存失敗: {ex.Message}");
			}
		}

		/* 取得行程清單 */
		[HttpGet("/api/trips/list")]
		[Authorize(AuthenticationSchemes = "CustomerAuth")]
		public async Task<IActionResult> GetTripList(int customerId)
		{
			var trips = await _context.CustomerTripProjects
				.Where(t => t.CustomerID == customerId)
				.OrderByDescending(t => t.UpdateTime)
				.Select(t => new
				{
					t.ProjectID,
					t.ProjectName,
					t.TotalDays,
					t.CreateTime,
					t.UpdateTime
				}).ToListAsync();

			return Json(trips);
		}
		
		/* 取得單筆行程明細 */
		[HttpGet("/api/trips/{projectId}")]
		[Authorize(AuthenticationSchemes = "CustomerAuth")]
		public async Task<IActionResult> GetTripById(int projectId)
		{
			try
			{
				// 先查詢主表與相關關聯資料
				var project = await _context.CustomerTripProjects
					.Where(p => p.ProjectID == projectId)
					.Include(p => p.TripProjectDetails)
						.ThenInclude(d => d.Location)
					.Include(p => p.TripProjectDetails)
						.ThenInclude(d => d.Hotel)
					.Include(p => p.TripProjectDetails)
						.ThenInclude(d => d.Restaurant)
					.FirstOrDefaultAsync();

				if (project == null)
					return NotFound("找不到行程資料");

				// 將明細資料轉為可安全輸出的 ViewModel
				var details = project.TripProjectDetails
					.OrderBy(d => d.TripDate)
					.ThenBy(d => d.TripSequence)
					.Select(d => new TripProjectViewModel
					{
						TripDate = d.TripDate,
						TripSequence = d.TripSequence,
						// 安全轉換 TimeSpan → 字串
						StartTime = d.StartTime.HasValue
							? d.StartTime.Value.ToString(@"hh\:mm")  // 注意「單斜線」格式
							: "",
						StayMinute = d.StayMinute,
						TripType = d.TripType,
						HotelID = d.HotelID,
						LocationID = d.LocationID,
						RestaurantID = d.RestaurantID,
						Notes = d.Notes,
						Hotel = d.Hotel,
						Location = d.Location,
						Restaurant = d.Restaurant
					}).ToList();

				// 組合主檔 + 明細一起輸出
				var result = new TripProjectViewModel
				{
					ProjectID = project.ProjectID,
					ProjectName = project.ProjectName,
					TotalDays = project.TotalDays,
					CustomerID = project.CustomerID,
					Details = details
				};

				// CamelCase 輸出（Vue 可直接接）
				return new JsonResult(result, new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});
			}
			catch (FormatException fex)
			{
				// 若特定欄位格式錯誤（TimeSpan 轉換失敗等）
				return StatusCode(500, $"時間格式錯誤：{fex.Message}\n🔍 堆疊：{fex.StackTrace}");
			}
			catch (Exception ex)
			{
				// 其他例外情況
				return StatusCode(500, $"錯誤訊息：{ex.Message}\n📂 來源：{ex.Source}\n🔍 堆疊：{ex.StackTrace}");
			}
		}

		/* 刪除行程 */
		[HttpDelete("/api/trips/{projectId}")]
		[Authorize(AuthenticationSchemes = "CustomerAuth")]
		public async Task<IActionResult> DeleteTrip(int projectId)
		{
			var project = await _context.CustomerTripProjects
				.Include(p => p.TripProjectDetails)
				.FirstOrDefaultAsync(p => p.ProjectID == projectId);

			if (project == null)
				return NotFound("找不到行程");

			_context.TripProjectDetails.RemoveRange(project.TripProjectDetails);
			_context.CustomerTripProjects.Remove(project);
			await _context.SaveChangesAsync();

			return Ok("行程已刪除");
		}

		/* 更新（覆寫）行程 */
		[HttpPut("/api/trips/update")]
		[Authorize(AuthenticationSchemes = "CustomerAuth")]
		public async Task<IActionResult> UpdateTrip([FromBody] TripProjectViewModel data)
		{
			var project = await _context.CustomerTripProjects
				.Include(p => p.TripProjectDetails)
				.FirstOrDefaultAsync(p => p.ProjectID == data.ProjectID);

			if (project == null)
				return NotFound("找不到要更新的行程");

			project.ProjectName = data.ProjectName ?? project.ProjectName;
			project.TotalDays = data.TotalDays ?? project.TotalDays;
			project.UpdateTime = DateTime.Now;

			_context.TripProjectDetails.RemoveRange(project.TripProjectDetails);

			var details = data.Details.Select(d =>
			{
				TimeSpan parsedTime = TimeSpan.Zero;
				if (!string.IsNullOrWhiteSpace(d.StartTime))
					TimeSpan.TryParse(d.StartTime, out parsedTime);

				return new TripProjectDetails
				{
					ProjectID = project.ProjectID,
					TripDate = d.TripDate,
					TripSequence = d.TripSequence ?? 0,
					StartTime = parsedTime,
					StayMinute = d.StayMinute ?? 0,
					TripType = d.TripType,
					HotelID = d.HotelID,
					LocationID = d.LocationID,
					RestaurantID = d.RestaurantID,
					Notes = d.Notes
				};
			}).ToList();

			_context.TripProjectDetails.AddRange(details);
			await _context.SaveChangesAsync();

			return Ok("行程已更新");
		}

		[HttpPost("/api/views/increase")]
		[AllowAnonymous] // ✅ 不需登入即可記錄瀏覽數
		public async Task<IActionResult> IncreaseView(string type, int id)
		{
			try
			{
				switch (type.ToLower())
				{
					case "locations":
						var loc = await _context.Locations.FindAsync(id);
						if (loc == null) return NotFound("找不到景點資料");
						loc.Views = (loc.Views ?? 0) + 1; // ✏️ 累加瀏覽數
						break;

					case "restaurants":
						var res = await _context.Restaurants.FindAsync(id);
						if (res == null) return NotFound("找不到美食資料");
						res.Views = (res.Views ?? 0) + 1;
						break;

					case "hotels":
						var hotel = await _context.Hotels.FindAsync(id);
						if (hotel == null) return NotFound("找不到住宿資料");
						hotel.Views = (hotel.Views ?? 0) + 1;
						break;

					default:
						return BadRequest("❌ 類型參數錯誤（需為 locations / restaurants / hotels）");
				}

				// 儲存異動
				await _context.SaveChangesAsync();

				return Ok("✅ 瀏覽次數已更新");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"❌ 更新失敗：{ex.Message}");
			}
		}

		/* ============================================================
   🟡 更新星星評分 Rating
   ------------------------------------------------------------
   說明：
   - 當使用者在前端點擊星星時呼叫此 API
   - 採「單筆平均法」：(舊分數 + 新分數) / 2
   - 因資料庫欄位為 numeric(2,1)，EF Core 對應 decimal 型別
   - 所以要使用 decimal 計算，避免 double 混用錯誤
   ============================================================ */
		[HttpPost("/api/rating/update")]
		[Authorize(AuthenticationSchemes = "CustomerAuth")] // ✅ 需登入會員才能評分
		public async Task<IActionResult> UpdateRating(string type, int id, double score)
		{
			try
			{
				switch (type.ToLower())
				{
					// ------------------------------------------------------------
					// 🏞️ 景點 (Locations)
					// ------------------------------------------------------------
					case "locations":
						{
							var loc = await _context.Locations.FindAsync(id);
							if (loc == null) return NotFound("找不到景點資料");

							decimal newScore = Convert.ToDecimal(score); // ✅ double → decimal

							if (loc.Rating.HasValue)
								loc.Rating = System.Math.Round(((loc.Rating.Value + newScore) / 2), 1);
							else
								loc.Rating = System.Math.Round(newScore, 1);
							break;
						}

					// ------------------------------------------------------------
					// 🍜 美食 (Restaurants)
					// ------------------------------------------------------------
					case "restaurants":
						{
							var res = await _context.Restaurants.FindAsync(id);
							if (res == null) return NotFound("找不到美食資料");

							decimal newScore = Convert.ToDecimal(score); // ✅ double → decimal

							if (res.Rating.HasValue)
								res.Rating = System.Math.Round(((res.Rating.Value + newScore) / 2), 1);
							else
								res.Rating = System.Math.Round(newScore, 1);
							break;
						}

					// ------------------------------------------------------------
					// 🏨 住宿 (Hotels)
					// ------------------------------------------------------------
					case "hotels":
						{
							var hotel = await _context.Hotels.FindAsync(id);
							if (hotel == null) return NotFound("找不到住宿資料");

							decimal newScore = Convert.ToDecimal(score); // ✅ double → decimal

							if (hotel.Rating.HasValue)
								hotel.Rating = System.Math.Round(((hotel.Rating.Value + newScore) / 2), 1);
							else
								hotel.Rating = System.Math.Round(newScore, 1);
							break;
						}

					// ------------------------------------------------------------
					// ❌ 類型錯誤
					// ------------------------------------------------------------
					default:
						return BadRequest("❌ 類型參數錯誤（需為 locations / restaurants / hotels）");
				}

				// ✅ 儲存更新
				await _context.SaveChangesAsync();
				return Ok("⭐ 評分已更新！");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"❌ 更新評分失敗：{ex.Message}");
			}
		}



	}
}
