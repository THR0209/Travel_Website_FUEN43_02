using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerSupportFeedback
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
