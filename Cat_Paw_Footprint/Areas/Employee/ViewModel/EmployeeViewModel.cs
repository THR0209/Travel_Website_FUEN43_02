namespace Cat_Paw_Footprint.Areas.Employee.ViewModel
{
	public class EmployeeViewModel
	{
		public int? EmployeeID { get; set; }//員工編號
		public int? EmployeeProfileID { get; set; }//員工資料編號
		public string? Account { get; set; }//帳號
		public string? Password { get; set; }//密碼
		public int? RoleID { get; set; }//角色編號
		public string? RoleName { get; set; }//角色名稱
		public DateTime? CreateDate { get; set; }//建立日期
		public bool? Status { get; set; }//帳號啟用狀態
		public string? EmployeeCode { get; set; }//員工代號
		public string? EmployeeProfileCode { get; set; }//員工資料代號
		public string? EmployeeName { get; set; }//員工姓名
		public string? IDNumber { get; set; }//身分證字號
		public string? Phone { get; set; }//電話
		public string? Email { get; set; }//電子郵件
		public string? Address { get; set; }//地址
		public byte[]? Photo { get; set; }//照片
		public string? ErrorMessage { get; set; }//錯誤訊息
	}
}
