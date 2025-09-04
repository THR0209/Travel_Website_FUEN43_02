using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerOrderMemberInfo
{
    public int? OrderID { get; set; }

    public string? CustomerName { get; set; }

    public string? IDNumber { get; set; }

    public DateTime? CustomerBirth { get; set; }

    public virtual CustomerOrders? Order { get; set; }
}
