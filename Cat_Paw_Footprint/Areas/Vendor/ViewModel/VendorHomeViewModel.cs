namespace Cat_Paw_Footprint.Areas.Vendor.ViewModel
{
	public class VendorHomeViewModel
	{
		public int VendorId { get; set; }//廠商編號
		public string UserId { get; set; } = null!;// AspNetUsers.Id
		public string Account { get; set; } = null!;//廠商帳號
		public string CompanyName { get; set; } = null!;//廠商名稱
		public string? Email { get; set; }//聯絡信箱
		public string? ContactName { get; set; }//聯絡人
		public string? Phone { get; set; }//聯絡電話
		public string? Address { get; set; }//公司地址
		public string? TaxId { get; set; }//統編
		public bool Status { get; set; }//啟用/停用
		public DateTime CreateDate { get; set; }//建立日期
		public DateTime? UpdateDate { get; set; }//廠商資料最後更新日期
		public int LoginLogID { get; set; }//登入紀錄編號
		public DateTime LoginTime { get; set; }//登入時間
		public bool IsSuccessful { get; set; }//是否登入成功
		public string? LoginIP { get; set; }// 登入 IP (IPv4/IPv6)
		public string? Message { get; set; }// 額外訊息(資料庫表單沒有用來傳遞回傳結果)
	}
}
