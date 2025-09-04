using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class PaymentStatus
{
	[Key]
	public int PayMentStatusID { get; set; }

    public string? StatusDesc { get; set; }

    public virtual ICollection<OrderPaymentInfo> OrderPaymentInfo { get; set; } = new List<OrderPaymentInfo>();
}
