using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerSupportMessages
{
	[Key]
	public int MessageID { get; set; }

    public int? TicketID { get; set; }

    public int? SenderID { get; set; }

    public int? ReceiverID { get; set; }

    public string? MessageContent { get; set; }

    public int? UnreadCount { get; set; }

    public string? AttachmentURL { get; set; }

    public DateTime? SentTime { get; set; }

    public virtual CustomerSupportTickets? Ticket { get; set; }
}
