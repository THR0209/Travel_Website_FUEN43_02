using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomerService.ViewModel
{
	// FAQ 與分類用於前端資料的 ViewModel
	public class FAQServiceDashboardViewModel
	{
		// FAQ 與 FAQ 分類前端資料用的 ViewModel
		public class FAQViewModel
		{
			// FAQ 主鍵
			public int FAQID { get; set; }
			[Required(ErrorMessage = "問題不可為空")]
			[StringLength(200, ErrorMessage = "問題長度不可超過 200 字")]
			public string? Question { get; set; }

			[Required(ErrorMessage = "答案不可為空")]
			public string? Answer { get; set; }

			[Required(ErrorMessage = "請選擇分類")]
			public int? CategoryID { get; set; }

			public string? CategoryName { get; set; }
			public bool IsActive { get; set; } = false;
			public DateTime? CreateTime { get; set; }
			public DateTime? UpdateTime { get; set; }
		}

		// FAQ 分類資料的 ViewModel
		public class FAQCategoryViewModel
		{
			public int? CategoryID { get; set; }

			[Required(ErrorMessage = "分類名稱不可為空")]
			[StringLength(50, ErrorMessage = "分類名稱長度不可超過 50 字")]
			public string? CategoryName { get; set; }
		}
	}
}