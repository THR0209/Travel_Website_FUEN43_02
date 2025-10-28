using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
    [Area("CustomersArea")]
    [Route("CustomersArea/[controller]")]
    [Authorize(AuthenticationSchemes = "CustomerAuth")]
    public class CouponsController : Controller
    {
        private readonly webtravel2Context _db;
        public CouponsController(webtravel2Context db) => _db = db;

        private int CurrentCustomerId =>
            int.TryParse(User.FindFirst("CustomerId")?.Value, out var id) ? id : 0;

        // ★ 用這支：最嚴謹（JOIN + null-safe）
        [HttpGet("api")]
        public async Task<IActionResult> MyCoupons()
        {
            var cid = CurrentCustomerId;
            if (cid <= 0) return Unauthorized();

            var now = DateTime.Now;

            // 1) 客戶等級（nullable → 預設 0）
            var level = await _db.Customers
                .Where(c => c.CustomerID == cid)
                .Select(c => (int?)(c.Level) ?? 0)
                .FirstOrDefaultAsync();

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

            // 4) 合併 + 去重（以 CouponId 為主）
            var listByRecord = await qByRecord.ToListAsync();
            var listByLevel = await qByLevel.ToListAsync();
            var merged = listByRecord
                .Concat(listByLevel)
                .GroupBy(x => x.CouponId)
                .Select(g => g.First())
                .OrderBy(x => x.ExpireAt ?? DateTime.MaxValue)
                .ToList();

            return Ok(merged);
        }

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
