using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerSupportTickets
{
	[Key]
	public int TicketID { get; set; }

    public int? CustomerID { get; set; }

    public int? EmployeeID { get; set; }

    public string Subject { get; set; } = null!;

    public int? TicketTypeID { get; set; }

    public string? Description { get; set; }

    public int? StatusID { get; set; }

    public int? PriorityID { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public virtual CustomerProfile? Customer { get; set; }

    public virtual ICollection<CustomerSupportFeedback> CustomerSupportFeedback { get; set; } = new List<CustomerSupportFeedback>();

	public virtual ICollection<CustomerSupportMessages> Messages { get; set; } = new List<CustomerSupportMessages>();

	public virtual Employees? Employee { get; set; }

    public virtual TicketPriority? Priority { get; set; }

    public virtual TicketStatus? Status { get; set; }

    public virtual TicketTypes? TicketType { get; set; }
}
