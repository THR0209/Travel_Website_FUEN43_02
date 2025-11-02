using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[AllowAnonymous]
	public class TravelController : Controller
	{
		private readonly webtravel2Context _context;    // 資料庫 Context
		private readonly IConfiguration _config;    // appsettings.json 設定檔

		//	建構子：注入資料庫 Context
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
					// 🟢 嘗試將 "12:30" 轉成 TimeSpan(12,30,0)
					TimeSpan parsedTime = TimeSpan.Zero;
					if (!string.IsNullOrWhiteSpace(d.StartTime))
						TimeSpan.TryParse(d.StartTime, out parsedTime);

					return new TripProjectDetails
					{
						ProjectID = project.ProjectID,
						TripDate = d.TripDate,
						TripSequence = d.TripSequence ?? 0,
						StartTime = parsedTime,                // ✅ 安全轉換後使用
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
				return Ok(new
				{
					message = "✅ 行程已成功儲存！",
					projectId = project.ProjectID,
					count = details.Count
				});
			}
			catch (Exception ex)
			{
				// 若有錯誤，回傳 500 錯誤碼與訊息
				return StatusCode(500, $"❌ 儲存失敗: {ex.Message}");
			}
		}

	}
}
