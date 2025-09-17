using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cat_Paw_Footprint.Areas.TravelManagement.ViewModel
{
	public class HotelsViewModel
	{
		[Display(Name ="住宿ID")]
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

		[Display(Name = "評分")]
		public decimal? Rating { get; set; }

		[Display(Name = "瀏覽數")]
		public int? Views { get; set; }

		[Display(Name = "住宿代碼")]
		public string? HotelCode { get; set; }

		[Display(Name = "是否啟用")]
		public bool? IsActive { get; set; }

		[NotMapped] // 不會影響資料庫
		public string IsActiveText => (bool)IsActive ? "啟用" : "停用";

		//多張圖片，用 IFormFile 來接收
		[Display(Name = "住宿圖片")]
		public List<IFormFile>? Picture { get; set; } = new List<IFormFile>();

		// 圖片的 Base64 字串清單（顯示用）
		public List<string> PictureBase64 { get; set; } = new();

		// 舊圖片的 ID，用來刪除對應圖片
		public List<int>? PictureIds { get; set; }

		//要刪掉的圖片 ID (在修改中使用)
		[NotMapped]
		public List<int>? DeletedPictureIds { get; set; }

		// 多個關鍵字
		[Display(Name = "關鍵字")]
		public List<int> KeywordID { get; set; } = new();

		// 關鍵字名稱清單（中文顯示用）
		public List<string>? KeywordNames { get; set; } = new();

		

		

	}
}
