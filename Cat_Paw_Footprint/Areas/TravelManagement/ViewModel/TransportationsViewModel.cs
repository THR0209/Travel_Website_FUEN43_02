using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cat_Paw_Footprint.Areas.TravelManagement.ViewModel
{
	public class TransportationsViewModel
	{
		public int TransportID { get; set; }

		[Display(Name = "交通名稱")]
		public string? TransportName { get; set; }

		[Display(Name = "交通介紹")]
		public string? TransportDesc { get; set; }

		[Display(Name = "交通價位")]
		public int? TransportPrice { get; set; }

		[Display(Name = "評分")]
		public decimal? Rating { get; set; }

		[Display(Name = "瀏覽數")]
		public int? Views { get; set; }

		[Display(Name = "交通代碼")]
		public string? TransportCode { get; set; }

		[Display(Name = "是否啟用")]
		public bool? IsActive { get; set; }

		[NotMapped] // 不會影響資料庫
		public string IsActiveText => (bool)IsActive ? "啟用" : "停用";

		//多張圖片，用 IFormFile 來接收
		[Display(Name = "交通圖片")]
		public List<IFormFile>? Picture { get; set; } = new List<IFormFile>();

		// 圖片Base64字串清單（顯示用）
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
