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

            // 2️⃣ 先抓出會員的所有優惠券 ID 與使用紀錄
            var customerCoupons = _context.CustomerCouponsRecords
                .Where(r => r.CustomerID == customerId)
                .Select(r => new { r.CouponID, r.IsUsed, r.ExpireTime })
                .ToList();

            var now = DateTime.Now;

            // 3️⃣ 撈出優惠券資料並組合資訊
            var coupons = (from r in customerCoupons
                           join c in _context.Coupons on r.CouponID equals c.CouponID
                           where c.IsActive == true
                           select new
                           {
                               CouponID = c.CouponID,
                               CouponName = c.CouponName,
                               Desc = c.CouponDesc,
                               DiscountType = c.DiscountType,
                               DiscountValue = c.DiscountValue,
                               MinimumAmount = c.MinimumAmount,
                               StartDate = c.StartDate,
                               EndDate = r.ExpireTime, // ✅ 改成會員個別有效期限
                               IsUsed = r.IsUsed,
                               IsExpired = r.ExpireTime < now, // ✅ 改成依據個人 ExpireTime 判斷
                               Rule = c.MinimumAmount != null
                                   ? (c.DiscountType == 1
                                       ? $"訂購金額須滿 TWD {c.MinimumAmount:N0} 打 {(c.DiscountValue * 10):0.##} 折"
                                           + (c.MaximumDiscount != null ? $"，最高折抵 {c.MaximumDiscount:N0} 元" : "")
                                       : $"訂購金額須滿 TWD {c.MinimumAmount:N0} 折 {c.DiscountValue:N0} 元")
                                   : (c.DiscountType == 1
                                       ? $"打 {(c.DiscountValue * 10):0.##} 折"
                                           + (c.MaximumDiscount != null ? $"，最高折抵 {c.MaximumDiscount:N0} 元" : "")
                                       : $"折 {c.DiscountValue:N0} 元")
                           }).ToList();

            // 4️⃣ 分類回傳
            var usable = coupons.Where(c => !(bool)c.IsUsed && !c.IsExpired).ToList();
            var used = coupons.Where(c => (bool)c.IsUsed).ToList();
            var expired = coupons.Where(c => !(bool)c.IsUsed && c.IsExpired).ToList();

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
