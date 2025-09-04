using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class OrderPaymentInfo
{
	[Key]
	public int PaymentID { get; set; }

    public int? OrderID { get; set; }

    public int? PaymentAmount { get; set; }

    public int? PaymentStatusID { get; set; }

    public DateTime? TransectionTime { get; set; }

    public string? Notes { get; set; }

    public virtual CustomerOrders? Order { get; set; }

    public virtual PaymentStatus? PaymentStatus { get; set; }
}
