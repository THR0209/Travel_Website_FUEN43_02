using Cat_Paw_Footprint.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cat_Paw_Footprint.Models;

[DateRange("StartDate", "EndDate", ErrorMessage = "啟用時間不可晚於到期時間")]
public partial class Coupons
{
	public int CouponID { get; set; }

	[DisplayName("流水號")]
	public string? CouponCode { get; set; }

	[DisplayName("內容描述")]
	public string? CouponDesc { get; set; }

	[DisplayName("折扣形式")]
	public int? DiscountType { get; set; }

	[DisplayName("折扣數值")]
	public decimal? DiscountValue { get; set; }

	[DisplayName("折扣代碼")]
	public string? DisCountCode { get; set; }

	[DisplayName("啟用時間")]
	public DateTime? StartDate { get; set; }

	[DisplayName("結束時間")]
	public DateTime? EndTime { get; set; }

	[DisplayName("狀態")]
	public bool IsActive { get; set; }

	public ICollection<CouponPics> CouponPics { get; set; } = new List<CouponPics>();

	public ICollection<Coupon_CustomerLevels> Coupon_CustomerLevels { get; set; } = new List<Coupon_CustomerLevels>();
}
