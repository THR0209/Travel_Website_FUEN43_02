using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
		[Required(ErrorMessage = "折扣形式必須選擇")]
        public int DiscountType { get; set; }

		[DisplayName("折扣數值")]
        [Required(ErrorMessage = "折扣數值不可為空")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "折扣數值必須是數字")]
        [DiscountValueRange]
        public decimal DiscountValue { get; set; }

		[DisplayName("啟用時間")]
        [Required(ErrorMessage = "啟用時間不可為空")]
        public DateTime StartDate { get; set; }

		[DisplayName("結束時間")]
        [Required(ErrorMessage = "結束時間不可為空")]
        public DateTime EndDate { get; set; }

		[DisplayName("是否啟用")]
		public bool IsActive { get; set; }

		[DisplayName("折扣碼")]
		public string? DiscountCode { get; set; }

		[DisplayName("優惠券名稱")]
        [Required(ErrorMessage = "優惠券名稱不可為空")]
        public string CouponName { get; set; } = null!;

        [DisplayName("滿額門檻")]
        public decimal? MinimumAmount { get; set; }      // 滿額門檻

        [DisplayName("折扣上限")]
        public decimal? MaximumDiscount { get; set; }    // 折扣上限

        [DisplayName("可使用次數")]
        public int? UsageLimit { get; set; }             // 可使用次數
        public bool? PerOrderLimit { get; set; }         // 每筆訂單限用一次

        public string? TargetType { get; set; }

		public ICollection<CouponPics> CouponPics { get; set; } = new List<CouponPics>();

		public ICollection<Coupon_CustomerLevels> Coupon_CustomerLevels { get; set; } = new List<Coupon_CustomerLevels>();
	}
}
