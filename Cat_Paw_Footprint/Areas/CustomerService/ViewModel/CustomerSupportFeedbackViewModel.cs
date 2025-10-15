using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomerService.ViewModel
{
	/// <summary>
	/// 客戶服務評價前端用 ViewModel，包含評價明細與關聯工單
	/// </summary>
	public class CustomerSupportFeedbackViewModel
	{
		/// <summary>評價主鍵</summary>
		[Key]
		public int FeedbackID { get; set; }

		/// <summary>所屬工單 ID</summary>
		public int? TicketID { get; set; }

		/// <summary>客戶 ID</summary>
		public int? CustomerID { get; set; }

		/// <summary>評分（如 1~5）</summary>
		public int? FeedbackRating { get; set; }

		/// <summary>評價留言</summary>
		public string? FeedbackComment { get; set; }

		/// <summary>建立時間</summary>
		public DateTime? CreateTime { get; set; }

		/// <summary>關聯工單物件</summary>
		public virtual CustomerSupportTickets? Ticket { get; set; }
	}
}