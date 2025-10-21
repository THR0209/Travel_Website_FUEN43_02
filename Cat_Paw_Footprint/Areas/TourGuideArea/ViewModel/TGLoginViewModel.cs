using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel
{
	public class TGLoginViewModel
	{
		[Required(ErrorMessage = "帳號不得為空")]
		public string? Account { get; set; }

		[Required(ErrorMessage = "密碼輸入錯誤")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		public string? ErrorMessage { get; set; }
	}
}
