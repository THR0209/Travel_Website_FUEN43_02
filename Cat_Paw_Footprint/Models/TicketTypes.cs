using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class TicketTypes
{
	[Key]
	public int TicketTypeID { get; set; }

    public string? TicketTypeName { get; set; }

    public virtual ICollection<CustomerSupportTickets> CustomerSupportTickets { get; set; } = new List<CustomerSupportTickets>();
}
