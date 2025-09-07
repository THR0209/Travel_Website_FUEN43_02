using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.Employee.ViewModel
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "請輸入帳號"), StringLength(50, MinimumLength = 6)]
		[Display(Name = "帳號")]
		public string Account { get; set; }

		[Required(ErrorMessage = "請輸入密碼"), StringLength(100, MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "密碼")]
		public string Password { get; set; }

		[Required(ErrorMessage = "請輸入姓名"), StringLength(10)]
		[Display(Name = "姓名")]
		public string EmployeeName { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "請選擇角色")]
		[Display(Name = "角色")]
		public int RoleId { get; set; }

		public string ErrorMessage { get; set; } = string.Empty;
	}
}
