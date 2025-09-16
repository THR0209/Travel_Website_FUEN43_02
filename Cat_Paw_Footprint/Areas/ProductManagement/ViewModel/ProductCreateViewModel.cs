using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cat_Paw_Footprint.Areas.ProductManagement.ViewModel
{
	public class ProductCreateViewModel
	{
		// 基本欄位（對應 Products）
		//public int? ProductID { get; set; }            // 編輯時用
		//public string? ProductCode { get; set; }       // 你的系統會產生
		//[Required(ErrorMessage = "請輸入產品名稱")] public string ProductName { get; set; } = "";
		///*[Required(ErrorMessage = "請選擇出團地區")]*/ public int? RegionID { get; set; }
		///*[Required]*/ public string? ProductDesc { get; set; }
		///*[Required]*/ public decimal? ProductPrice { get; set; }
		//public string? ProductNote { get; set; }
		//public DateTime? StartDate { get; set; }
		//public DateTime? EndDate { get; set; }
		//public int? MaxPeople { get; set; }
		//public byte[]? ProductImage { get; set; }      // 顯示用

		public Products Product { get; set; } = new Products();

		public ProductAnalysis ProductAnalysis { get; set; } = new();

		[NotMapped] //不進資料庫
		public IFormFile? UploadImage { get; set; }    // 上傳用

		// 多對多：交通方式、景點、飯店、餐廳（橋接表 Products_Transportations）
		public List<int> SelectedTransportationIds { get; set; } = new List<int>();
		public List<int> SelectedLocationIds { get; set; } = new List<int>();
		public List<OrderedItem?> SelectedHotelIds { get; set; } = new();
		public List<int> SelectedRestaurantIds { get; set; } = new List<int>();

		public List<OrderedItem?> SelectedRestaurantsBreakfast { get; set; } = new();
		public List<OrderedItem?> SelectedRestaurantsLunch { get; set; } = new();
		public List<OrderedItem?> SelectedRestaurantsDinner { get; set; } = new();

		public Dictionary<int, List<OrderedItem>> SelectedLocationsByDay { get; set; } = new();

		public List<DayLocations> SelectedLocations { get; set; } = new();


	}

	public class RestaurantSelection
	{
		public int? RestaurantID { get; set; }
		public string? MealType { get; set; }
	}

	public class OrderedItem
	{
		public int? ID { get; set; }
		public int? OrderIndex { get; set; }
	}

	public class DayLocations
	{
		public int DayNumber { get; set; }
		public List<OrderedItem> Locations { get; set; } = new();
	}


}
