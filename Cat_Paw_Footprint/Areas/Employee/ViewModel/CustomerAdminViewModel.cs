namespace Cat_Paw_Footprint.Areas.Employee.ViewModel
{
	public class CustomerAdminViewModel
	{
		public int? CustomerID { get; set; }//客戶編號
		public int? CustomerProfileID { get; set; }//客戶資料編號
		public string? Account { get; set; }//客戶帳號
		public DateTime? CreateDate { get; set; }//建立日期
		public bool? IsBlacklisted { get; set; }//是否黑名單
		public bool? Status { get; set; }//帳號啟用狀態
		public string? CustomerCode { get; set; }//客戶代號
		public string? CustomerProfileCode { get; set; }//客戶資料代號
		public string? CustomerName { get; set; }//客戶姓名//等同於Customers表的FullName
		public string? IDNumber { get; set; }//身分證字號
		public string? Phone { get; set; }//電話
		public string? Email { get; set; }//電子郵件
		public string? Address { get; set; }//地址
		public string? UserId { get; set; }// ★ 新增：指向 AspNetUsers(Id)
		public int? Level { get; set; }//會員等級
		public string? LevelName { get; set; }//會員等級名稱
		public int LoginLogID { get; set; }//登入紀錄編號
		public string? LoginIP { get; set; }//登入IP
		public DateTime? LoginTime { get; set; }//登入時間
		public bool? IsSuccessful { get; set; }//是否登入成功
		public string? ErrorMessage { get; set; }//錯誤訊息
	}
}
