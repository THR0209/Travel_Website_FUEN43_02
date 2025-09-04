using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.Employee.ViewModel
{
	public class RegisterViewModel
	{
		[Required, StringLength(50)]
		public string Account { get; set; }

		[Required, StringLength(100, MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required, StringLength(50)]
		public string EmployeeName { get; set; }
		[Range(1, int.MaxValue, ErrorMessage = "請選擇角色")]
		public int RoleId { get; set; }

		public string ErrorMessage { get; set; } = string.Empty;
	}
}
