using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.Employee.ViewModel
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "請輸入帳號")]
		[StringLength(50, ErrorMessage = "帳號長度不可超過 50 個字元")]
		[Display(Name = "帳號")]
		public string Account { get; set; }

		[Required(ErrorMessage = "請輸入密碼")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於 6 到 100 個字元之間")]
		[Display(Name = "密碼")]
		public string Password { get; set; }

		[Required(ErrorMessage = "請輸入姓名")]
		[StringLength(20, ErrorMessage = "姓名長度不可超過 30 個字元")]
		[Display(Name = "姓名(中英文皆可)")]
		public string EmployeeName { get; set; }

		[Required(ErrorMessage = "請選擇角色")]
		[Range(1, int.MaxValue, ErrorMessage = "請選擇角色")]
		[Display(Name = "角色")]
		public int? RoleId { get; set; }

		public string ErrorMessage { get; set; } = string.Empty;
	}
}
