namespace Cat_Paw_Footprint.Areas.CustomerService.ViewModel
{
	/// <summary>
	/// 客戶服務訊息前端用 ViewModel，包含訊息內容及發送者資訊
	/// </summary>
	public class CustomerSupportMessageViewModel
	{
		/// <summary>訊息主鍵</summary>
		public int MessageID { get; set; }
		
		/// <summary>所屬工單 ID</summary>
		public int? TicketID { get; set; }
		
		/// <summary>發送者 ID</summary>
		public int? SenderID { get; set; }
		
		/// <summary>接收者 ID</summary>
		public int? ReceiverID { get; set; }
		
		/// <summary>訊息內容</summary>
		public string? MessageContent { get; set; }
		
		/// <summary>未讀計數</summary>
		public int? UnreadCount { get; set; }
		
		/// <summary>附件網址</summary>
		public string? AttachmentURL { get; set; }
		
		/// <summary>發送時間</summary>
		public DateTime? SentTime { get; set; }
		
		/// <summary>發送者角色（員工/客戶/未知）</summary>
		public string? SenderRole { get; set; }
		
		/// <summary>發送者顯示名稱（員工或客戶名字）</summary>
		public string? SenderDisplayName { get; set; }

		/// <summary>拿來對應前端暫存 ID 用的欄位，不會存到資料庫</summary>
		public string? TempId { get; set; }

		/// <summary>
		/// 拿來標記是客服端還是客戶端發送的訊息（"Customer" 或 "Admin"），方便前端區分顯示樣式
		/// </summary>
		public string SenderType { get; set; }   // "Customer" 或 "Admin"
		/// <summary>
		/// 用來顯示發送者名稱（例如客戶名稱或客服人員名稱）
		/// </summary>
		public string SentBy { get; set; }       // 發送者名稱
	}
}