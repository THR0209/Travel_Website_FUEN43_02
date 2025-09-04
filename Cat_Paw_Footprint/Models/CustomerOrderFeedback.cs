using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerOrderFeedback
{
	[Key]
	public int FeedbackID { get; set; }

    public int? OrderID { get; set; }

    public int? CustomerID { get; set; }

    public int? FeedbackRating { get; set; }

    public string? FeedbackComment { get; set; }

    public DateTime? CreateTime { get; set; }

    public virtual CustomerOrders? Order { get; set; }
}
