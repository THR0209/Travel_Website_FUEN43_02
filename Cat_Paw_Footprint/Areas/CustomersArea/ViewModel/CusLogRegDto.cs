namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	public class CusLogRegDto// 客戶登入註冊修改資料傳輸物件
	{
		public int CustomerId { get; set; }// 客戶編號
		public string UserId { get; set; } = null!;// AspNetUsers.Id
		public string Account { get; set; } = null!;// 客戶帳號
		public string? CustomerName { get; set; }// 客戶名稱等同FullName
		public string Password { get; set; } = null!;// 客戶密碼
		public string? Email { get; set; }// 客戶電子郵件
		public string? Phone { get; set; }// 客戶電話
		public string? Address { get; set; }// 客戶地址
		public string? CustomerCode { get; set; }// 客戶代碼
		public string IDNumber { get; set; } = null!;// 客戶身分證字號
		public int? Level { get; set; }// 客戶等級 
		public string? LevelName { get; set; }// 客戶等級名稱
		public DateTime? CreateDate { get; set; }// 建立日期
		public bool Status { get; set; }// 啟用/停用
		public int LoginLogID { get; set; }// 登入紀錄編號
		public DateTime? LoginTime { get; set; }// 登入時間
		public bool? IsSuccessful { get; set; }// 是否登入成功
		public bool? IsBlacklisted { get; set; }// 是否列入黑名單
		public string? LoginIP { get; set; }// 登入 IP (IPv4/IPv6)
		public string? FullName { get; set; }// 客戶全名
		public string ErrorMessage { get; set; } = string.Empty;// 錯誤訊息
		public string Message { get; set; } = string.Empty;// 一般訊息
	}
}
