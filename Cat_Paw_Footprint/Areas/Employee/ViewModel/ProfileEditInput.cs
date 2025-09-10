using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace Cat_Paw_Footprint.Areas.Employee.ViewModel
{
	public class ProfileEditInput
	{
		[Required(ErrorMessage = "姓名必填")]
		public string EmployeeName { get; set; } = null!;

		[Phone(ErrorMessage = "電話格式不正確")]
		public string? Phone { get; set; }

		[EmailAddress(ErrorMessage = "Email 格式不正確")]
		public string? Email { get; set; }

		public string? Address { get; set; }

		// 上傳檔案 → 存成 byte[]
		public IFormFile? PhotoFile { get; set; }
		public byte[]? ExistingPhoto { get; set; }

		// 舊密碼（使用者自己修改密碼時才會用到）
		public string? CurrentPassword { get; set; }
		public string? IDNumber { get; set; }//身分證字號
		public string? EmployeeProfileCode { get; set; }//員工資料代號

		// 新密碼（會在 Service 雜湊後傳給 Repo）
		[StringLength(100, MinimumLength = 6, ErrorMessage = "密碼至少 6 碼")]
		public string? NewPassword { get; set; }
	}
}
