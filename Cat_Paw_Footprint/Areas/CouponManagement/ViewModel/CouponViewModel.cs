using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.ValidationAttributes;
using System.ComponentModel;

namespace Cat_Paw_Footprint.Areas.CouponManagement.ViewModel
{
	[DateRange("StartDate", "EndDate", ErrorMessage = "啟用時間不可晚於結束時間")]
	public class CouponViewModel
	{
		public int CouponID { get; set; }

		[DisplayName("流水號")]
		public string? CouponCode { get; set; }

		[DisplayName("內容描述")]
		public string? CouponDesc { get; set; }

		[DisplayName("折扣形式")]
		public int DiscountType { get; set; }

		[DisplayName("折扣數值")]
		public decimal DiscountValue { get; set; }

		[DisplayName("啟用時間")]
		public DateTime StartDate { get; set; }

		[DisplayName("結束時間")]
		public DateTime EndDate { get; set; }

		[DisplayName("是否啟用")]
		public bool IsActive { get; set; }

		[DisplayName("折扣碼")]
		public string? DiscountCode { get; set; }

		[DisplayName("優惠券名稱")]
		public string CouponName { get; set; } = null!;

		public string? TargetType { get; set; }

		public ICollection<CouponPics> CouponPics { get; set; } = new List<CouponPics>();

		public ICollection<Coupon_CustomerLevels> Coupon_CustomerLevels { get; set; } = new List<Coupon_CustomerLevels>();
	}
}
