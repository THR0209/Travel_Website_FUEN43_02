using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.Employee.ViewModel
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "帳號不得為空")]
		public string? Account { get; set; }

		[Required(ErrorMessage = "密碼輸入錯誤")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		public string? ErrorMessage { get; set; }
	}
}
