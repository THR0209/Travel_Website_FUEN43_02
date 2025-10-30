using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public IActionResult GetMyCoupons()
        {
            // 1️⃣ 嘗試抓取登入會員的 CustomerId
            var customerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "CustomerId");
            if (customerIdClaim == null)
            {
                return Json(new { message = "尚未登入，請先登入會員後再查看優惠券。" });
            }

            if (!int.TryParse(customerIdClaim.Value, out var customerId))
            {
                return Json(new { message = "無法解析會員識別碼。" });
            }


          
            // 先抓出會員的所有優惠券 ID
            var couponIds = _context.CustomerCouponsRecords
                .Where(r => r.CustomerID == customerId)
                .Select(r => new { r.CouponID, r.IsUsed })
                .ToList();

            // 用 ID 去撈優惠券資料
            var coupons = (from r in couponIds
                           join c in _context.Coupons
                           on r.CouponID equals c.CouponID
                           select new
                           {
                               CouponID = c.CouponID,
                               Desc = c.CouponDesc,
                               DiscountValue = c.DiscountValue,
                               DiscountType = c.DiscountType,
                               StartDate = c.StartDate,
                               EndDate = c.EndDate,
                               IsUsed = r.IsUsed,
                               IsExpired = c.EndDate < DateTime.Now,
                               CouponName = c.CouponName,
                               MinimumAmount = c.MinimumAmount,
                               
                           }).ToList();





            // 3️⃣ 分類回傳（避免 null 問題）
            var usable = coupons.Where(c => !c.IsUsed && !c.IsExpired).ToList();
            var used = coupons.Where(c => c.IsUsed).ToList();
            var expired = coupons.Where(c => !c.IsUsed && c.IsExpired).ToList();

            return Json(new
            {
                usable,
                used,
                expired
            });
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
