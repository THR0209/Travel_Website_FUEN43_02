using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class TicketPriority
{
	[Key]
	public int PriorityID { get; set; }

    public string? PriorityDesc { get; set; }

    public virtual ICollection<CustomerSupportTickets> CustomerSupportTickets { get; set; } = new List<CustomerSupportTickets>();
}
