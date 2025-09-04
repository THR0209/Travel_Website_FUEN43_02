using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class TicketStatus
{
	[Key]
	public int StatusID { get; set; }

    public string? StatusDesc { get; set; }

    public virtual ICollection<CustomerSupportTickets> CustomerSupportTickets { get; set; } = new List<CustomerSupportTickets>();
}
