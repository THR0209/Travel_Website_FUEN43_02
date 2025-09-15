using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.TravelManagement.ViewModel
{
	public class RestaurantsViewModel
	{
		public int RestaurantID { get; set; }

		[Display(Name = "美食名稱")]
		public string? RestaurantName { get; set; }
		
		[Display(Name = "美食地址")]
		public string? RestaurantAddr { get; set; }

		[Display(Name = "經度")]
		public decimal? RestaurantLat { get; set; }

		[Display(Name = "緯度")]
		public decimal? RestaurantLng { get; set; }

		[Display(Name = "美食介紹")]
		public string? RestaurantDesc { get; set; }

		[Display(Name = "縣市")]
		public string? DistrictName { get; set; }

		[Display(Name = "地區")]
		public string? RegionName { get; set; }

		[Display(Name = "評分")]
		public decimal? Rating { get; set; }

		[Display(Name = "瀏覽數")]
		public int? Views { get; set; }

		[Display(Name = "美食代碼")]
		public string? RestaurantCode { get; set; }

		[Display(Name = "是否啟用")]
		public bool? IsActive { get; set; }
		
		// 多張圖片
		[Display(Name = "美食圖片")]
		public List<byte[]> Picture { get; set; } = new();

		// 多個關鍵字
		[Display(Name = "關鍵字")]
		public List<int> Keywords { get; set; } = new();
	}
}
