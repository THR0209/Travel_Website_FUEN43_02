using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CouponPics
{
    [Key]
    public int CouponPicID { get; set; } // 預計更改的

    public int? CouponID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Coupons? Coupon { get; set; }
}
