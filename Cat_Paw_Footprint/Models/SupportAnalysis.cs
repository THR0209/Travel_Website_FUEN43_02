using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class SupportAnalysis
{
    public int? DateID { get; set; }

    public int? TicketID { get; set; }

    public int? OpenedCount { get; set; }

    public int? ResolvedCount { get; set; }

    public int? RatingAverage { get; set; }

    public string? TopCategory { get; set; }

    public virtual DateDimension? Date { get; set; }

    public virtual CustomerSupportTickets? Ticket { get; set; }
}
