using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class CouponPics
{
    public int? CouponID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Coupons? Coupon { get; set; }
}
