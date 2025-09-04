using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerOrders
{
    public int? CustomerID { get; set; }

    public int? ProductID { get; set; }
	[Key]
	public int OrderID { get; set; }

    public int? OrderStatusID { get; set; }

    public int? TotalAmount { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public virtual CustomerProfile? Customer { get; set; }

    public virtual ICollection<CustomerOrderFeedback> CustomerOrderFeedback { get; set; } = new List<CustomerOrderFeedback>();

    public virtual ICollection<OrderPaymentInfo> OrderPaymentInfo { get; set; } = new List<OrderPaymentInfo>();

    public virtual OrderStatus? OrderStatus { get; set; }

    public virtual Products? Product { get; set; }
}
