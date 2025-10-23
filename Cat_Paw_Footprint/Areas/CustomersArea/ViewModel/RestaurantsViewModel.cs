using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
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
		public virtual Districts? District { get; set; }

		[Display(Name = "縣市")]
		public int? DistrictID { get; set; }

		[Display(Name = "縣市")]
		public string? DistrictName { get; set; }

		[Display(Name = "地區")]
		public virtual Regions? Region { get; set; }

		[Display(Name = "地區")]
		public int? RegionID { get; set; }

		[Display(Name = "地區")]
		public string? RegionName { get; set; }

		[Display(Name = "評分")]
		public decimal? Rating { get; set; }

		[Display(Name = "瀏覽數")]
		public int? Views { get; set; }

		//多張圖片，用 IFormFile 來接收
		[Display(Name = "美食圖片")]
		public List<IFormFile>? Picture { get; set; } = new List<IFormFile>();

		[Display(Name = "美食圖片")]
		public List<string>? PictureUrl { get; set; } = new List<string>();

	}
}
