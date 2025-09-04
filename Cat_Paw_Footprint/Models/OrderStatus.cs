using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class OrderStatus
{
	[Key]
	public int OrderStatusID { get; set; }

    public string? StatusDesc { get; set; }

    public virtual ICollection<CustomerOrders> CustomerOrders { get; set; } = new List<CustomerOrders>();
}
