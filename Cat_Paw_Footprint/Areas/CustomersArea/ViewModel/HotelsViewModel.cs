using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	public class HotelsViewModel
	{
		[Display(Name = "住宿ID")]
		public int HotelID { get; set; }

		[Display(Name = "住宿名稱")]
		public string? HotelName { get; set; }

		[Display(Name = "住宿地址")]
		public string? HotelAddr { get; set; }

		[Display(Name = "經度")]
		public decimal? HotelLat { get; set; }

		[Display(Name = "緯度")]
		public decimal? HotelLng { get; set; }

		[Display(Name = "住宿介紹")]
		public string? HotelDesc { get; set; }

		[Display(Name = "縣市")]
		public virtual Districts? District { get; set; }

		[Display(Name = "縣市")]
		public int? DistrictID { get; set; }   // 從 Districts 轉換過來		

		[Display(Name = "縣市")]
		public string? DistrictName { get; set; }

		[Display(Name = "地區")]
		public virtual Regions? Region { get; set; }

		[Display(Name = "地區")]
		public int? RegionID { get; set; }     // 從 Regions 轉換過來		

		[Display(Name = "地區")]
		public string? RegionName { get; set; }

		[Display(Name = "住宿圖片")]
		public List<string>? PictureUrl { get; set; } = new List<string>();

		//多張圖片，用 IFormFile 來接收
		[Display(Name = "住宿圖片")]
		public List<IFormFile>? Picture { get; set; } = new List<IFormFile>();
	}
}
