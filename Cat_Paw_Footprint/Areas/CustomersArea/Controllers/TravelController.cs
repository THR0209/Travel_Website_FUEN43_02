using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[AllowAnonymous]
	public class TravelController : Controller
	{
		//主頁面 View (顯示 Vue 畫面)
		public IActionResult Index()
		{
			return View();
		}

		//取得【住宿資料】（給 Vue3 呼叫用）
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
					PictureUrl = h.HotelPics.Select(p => p.PictureUrl).ToList()
				}).ToListAsync();

			return Json(data);
		}

		//取得【景點資料】（給 Vue3 呼叫用）
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
					PictureUrl = l.LocationPics.Select(p => p.PictureUrl).ToList()
				}).ToListAsync();

			return Json(data);
		}
		//取得【美食資料】（給 Vue3 呼叫用）
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
					PictureUrl = r.RestaurantPics.Select(p => p.PictureUrl).ToList()
				}).ToListAsync();

			return Json(data);
		}

	}
}
