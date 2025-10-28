using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;
using System.Security.Claims;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
    [Area("CustomersArea")]
    //[Route("CustomersArea/[controller]")]
    [Authorize(AuthenticationSchemes = "CustomerAuth")]
    public class CouponsController : Controller
    {
        private readonly webtravel2Context _context;
        public CouponsController(webtravel2Context context) => _context = context;

        private int CurrentCustomerId =>
            int.TryParse(User.FindFirst("CustomerId")?.Value, out var id) ? id : 0;

		public async Task<IActionResult> Index()
		{
            var ActiveCoupons = await _context.Coupons.Where(c => c.IsActive).ToListAsync();

			return View(ActiveCoupons);
		}

        public IActionResult GetMyCoupons()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(new { message = $"這是會員 {userId} 的優惠券資料" });
		}


            // 2) 客製券（未使用）
            var qByRecord = from r in _db.CustomerCouponsRecords
                            join cpn in _db.Coupons on r.CouponID equals cpn.CouponID
                            where r.CustomerID == cid
                               && (r.IsUsed == false || r.IsUsed == null)
                               && cpn.IsActive == true
                               && (cpn.StartDate == null || cpn.StartDate <= now)
                               && (cpn.EndDate == null || cpn.EndDate >= now)
                            select new CouponDto
                            {
                                CouponId = cpn.CouponID,
                                Code = cpn.DisCountCode!,    // 注意：資料表是 DisCountCode
                                Name = cpn.CouponDesc ?? "",
                                DiscountType = cpn.DiscountType ?? 0,
                                DiscountValue = cpn.DiscountValue ?? 0m,
                                ExpireAt = cpn.EndDate
                            };

            // 3) 等級券（符合我的 Level）
            var qByLevel = from map in _db.Coupon_CustomerLevels
                           join cpn in _db.Coupons on map.CouponID equals cpn.CouponID
                           where map.CustomerLevel == level
                              && cpn.IsActive == true
                              && (cpn.StartDate == null || cpn.StartDate <= now)
                              && (cpn.EndDate == null || cpn.EndDate >= now)
                           select new CouponDto
                           {
                               CouponId = cpn.CouponID,
                               Code = cpn.DisCountCode!,
                               Name = cpn.CouponDesc ?? "",
                               DiscountType = cpn.DiscountType ?? 0,
                               DiscountValue = cpn.DiscountValue ?? 0m,
                               ExpireAt = cpn.EndDate
                           };

		//	var coupons = await (
		//		from r in _context.CustomerCouponsRecords
		//		join cpn in _context.Coupons on r.CouponID equals cpn.CouponID
		//		where r.CustomerID == cid
		//			  && (r.IsUsed == false || r.IsUsed == null)
		//			  && cpn.IsActive == true
		//			  && (cpn.StartDate == null || cpn.StartDate <= now)
		//			  && (cpn.EndDate == null || cpn.EndDate >= now)
		//		select new CouponDto
		//		{
		//			CouponId = cpn.CouponID,
		//			Code = cpn.DiscountCode ?? "",
		//			Name = cpn.CouponName ?? cpn.CouponDesc ?? "",
		//			DiscountType = cpn.DiscountType,
		//			DiscountValue = cpn.DiscountValue,
		//			ExpireAt = cpn.EndDate
		//		}
		//	)
		//	.OrderBy(x => x.ExpireAt ?? DateTime.MaxValue)
		//	.ToListAsync();

		//	return Ok(coupons);
		//}


		public class CouponDto
        {
            public int CouponId { get; set; }
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";
            public int DiscountType { get; set; } // 0=金額、1=百分比（你的規格自己定）
            public decimal DiscountValue { get; set; }
            public DateTime? ExpireAt { get; set; }
        }
    }
}
