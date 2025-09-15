using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomerService.ViewModel
{
	public class CustomerSupportFeedbackViewModel
	{
		[Key]
		public int FeedbackID { get; set; }

		public int? TicketID { get; set; }

		public int? CustomerID { get; set; }

		public int? FeedbackRating { get; set; }

		public string? FeedbackComment { get; set; }

		public DateTime? CreateTime { get; set; }

		public virtual CustomerSupportTickets? Ticket { get; set; }
	}
}
