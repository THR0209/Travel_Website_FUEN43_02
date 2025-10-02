

namespace Cat_Paw_Footprint.Models;

public partial class Coupon_CustomerLevels
{
    public int CouponLevelID { get; set; }


	public int? CouponID { get; set; }

	public int? CustomerLevel { get; set; }

	public virtual Coupons? Coupon { get; set; }
}
