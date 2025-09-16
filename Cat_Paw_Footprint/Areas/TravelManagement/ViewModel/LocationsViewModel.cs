using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.TravelManagement.ViewModel
{
	public class LocationsViewModel
	{
		public int LocationID { get; set; }

		[Display(Name = "景點名稱")]
		public string? LocationName { get; set; }

		[Display(Name = "景點地址")]
		public string? LocationAddr { get; set; }

		[Display(Name = "經度")]
		public decimal? LocationLat { get; set; }

		[Display(Name = "緯度")]
		public decimal? LocationLng { get; set; }

		[Display(Name = "景點介紹")]
		public string? LocationDesc { get; set; }

		[Display(Name = "門票費用")]
		public int? LocationPrice { get; set; }

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

		[Display(Name = "景點代碼")]
		public string? LocationCode { get; set; }

		[Display(Name = "是否啟用")]
		public bool? IsActive { get; set; }

		// 多張圖片
		[Display(Name = "景點圖片")]
		public List<byte[]> Picture { get; set; } = new();

		// 多個關鍵字
		[Display(Name = "關鍵字")]
		public List<int> Keywords { get; set; } = new();
	}
}
