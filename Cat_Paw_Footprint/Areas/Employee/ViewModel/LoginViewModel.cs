using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.Employee.ViewModel
{
	public class LoginViewModel
	{
		[Required]
    public string? Account { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		public string? ErrorMessage { get; set; }
	}
}
