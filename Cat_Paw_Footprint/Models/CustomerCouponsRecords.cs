using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerCouponsRecords
{
    public int? CustomerID { get; set; }

    public int? CouponID { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? UsedTime { get; set; }

    public virtual Coupons? Coupon { get; set; }

    public virtual Customers? Customer { get; set; }
}
