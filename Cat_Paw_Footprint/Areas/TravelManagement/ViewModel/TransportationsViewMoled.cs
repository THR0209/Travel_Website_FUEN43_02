using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.TravelManagement.ViewModel
{
	public class TransportationsViewMoled
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

		// 多張圖片
		[Display(Name = "交通圖片")]
		public List<byte[]> Picture { get; set; } = new();

		// 多個關鍵字
		[Display(Name = "關鍵字")]
		public List<int> Keywords { get; set; } = new();
	}
}
