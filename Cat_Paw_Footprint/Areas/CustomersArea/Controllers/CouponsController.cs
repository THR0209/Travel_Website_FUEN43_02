using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
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

        public IActionResult GetMyCoupons()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var customerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "CustomerId");
			var CustomerId = int.Parse(customerIdClaim.Value);

			return Ok(new { message = $"這是會員 {CustomerId} 的優惠券資料" });
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
