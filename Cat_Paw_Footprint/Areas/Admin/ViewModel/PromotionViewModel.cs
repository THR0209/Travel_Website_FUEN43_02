using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.Admin.ViewModel
{
	public class PromotionViewModel
	{
		public int PromoID { get; set; }

		[Display(Name = "活動名稱")]
		public string? PromoName { get; set; }

		[Display(Name = "活動描述")]
		public string? PromoDesc { get; set; }

		[Display(Name = "開始時間")]
		public DateTime? StartTime { get; set; }

		[Display(Name = "結束時間")]
		public DateTime? EndTime { get; set; }

		[Display(Name = "折扣類型")]
		public int? DiscountType { get; set; }   // 1 = 百分比, 2 = 金額

		[Display(Name = "折扣數值")]
		public decimal? DiscountValue { get; set; }

		[Display(Name = "是否啟用")]
		public bool? IsActive { get; set; }

		[Display(Name = "建立時間")]
		public DateTime? CreateTime { get; set; }

		[Display(Name = "更新時間")]
		public DateTime? UpdateTime { get; set; }

		// 🔹 綁定的產品清單（詳細顯示）
		[Display(Name = "綁定產品")]
		public List<ProductViewModel> Products { get; set; } = new();

		// 🔹 選取產品用（新增/編輯時選 checkbox / multiselect）
		public List<int> SelectedProductIDs { get; set; } = new();
	}
}
