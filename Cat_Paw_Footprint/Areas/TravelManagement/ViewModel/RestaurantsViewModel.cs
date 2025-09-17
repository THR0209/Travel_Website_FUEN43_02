using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

		[Display(Name = "美食代碼")]
		public string? RestaurantCode { get; set; }

		[Display(Name = "是否啟用")]
		public bool? IsActive { get; set; }

		[NotMapped] // 不會影響資料庫
		public string IsActiveText => (bool)IsActive ? "啟用" : "停用";

		//多張圖片，用 IFormFile 來接收
		[Display(Name = "美食圖片")]
		public List<IFormFile>? Picture { get; set; } = new List<IFormFile>();

		//多張圖片，轉成 Base64 字串，用於顯示圖片
		public List<string> PictureBase64 { get; set; } = new();

		// 舊圖片的 ID，用來刪除對應圖片
		public List<int>? PictureIds { get; set; }

		//要刪掉的圖片 ID (在修改中使用)
		[NotMapped]
		public List<int> DeletedPictureIds { get; set; } = new();

		// 多個關鍵字
		[Display(Name = "關鍵字")]
		public List<int> KeywordID { get; set; } = new();

		// 關鍵字名稱清單（中文顯示用）
		public List<string>? KeywordNames { get; set; } = new();

	}
}
