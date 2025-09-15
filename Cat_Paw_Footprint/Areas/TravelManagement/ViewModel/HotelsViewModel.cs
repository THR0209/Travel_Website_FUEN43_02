using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.TravelManagement.ViewModel
{
	public class HotelsViewModel
	{
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
		public string? DistrictName { get; set; }   // 從 Districts 轉換過來

		[Display(Name = "地區")]
		public string? RegionName { get; set; }     // 從 Regions 轉換過來

		[Display(Name = "評分")]
		public decimal? Rating { get; set; }

		[Display(Name = "瀏覽數")]
		public int? Views { get; set; }

		[Display(Name = "住宿代碼")]
		public string? HotelCode { get; set; }

		[Display(Name = "是否啟用")]
		public bool? IsActive { get; set; }

		// 多張圖片
		[Display(Name = "住宿圖片")]
		public List<byte[]> Picture { get; set; } = new();

		// 多個關鍵字
		[Display(Name = "關鍵字")]
		public List<int> Keywords { get; set; } = new();

	}
}
