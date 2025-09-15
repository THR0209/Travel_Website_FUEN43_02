using Cat_Paw_Footprint.Areas.Admin.ViewModel;
using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ReportsController : Controller
	{
		private readonly webtravel2Context _context;

		public ReportsController(webtravel2Context context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			var now = DateTime.Now;
			var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
			var nextMonth = firstDayOfMonth.AddMonths(1);

			// 1. 本月訂單數 (CustomerOrders.CreateTime)
			var thisMonthOrders = _context.CustomerOrders
				.Count(o => o.CreateTime >= firstDayOfMonth && o.CreateTime < nextMonth);

			// 2. 開放行程數 (Products)
			var openTrips = _context.Products
				.Count(p => p.IsActive == true); // 假設 1 代表「開放中」

			// 3. 客戶總數 (Customers)
			var customerCount = _context.Customers.Count();

			// 4. 已解決案件數 (CustomerSupportTickets.StatusID 假設 3 = 已解決)
			var resolvedTickets = _context.CustomerSupportTickets
				.Count(t => t.StatusID == 3);

			var vm = new DashboardViewModel
			{
				MonthlyOrders = thisMonthOrders,
				OpenTrips = openTrips,
				CustomerCount = customerCount,
				ResolvedTickets = resolvedTickets
			};

			return View(vm);
		}

		// 1️⃣ 訂閱 vs 非訂閱比例
		[HttpGet]
		public IActionResult GetMemberSubscriptionRatio()
		{
			var result = _context.Customers
				.GroupBy(c => c.Level == null || c.Level == 0 ? "非訂閱會員" : "訂閱會員")
				.Select(g => new
				{
					Label = g.Key,
					Count = g.Count()
				})
				.ToList();

			// 保證有兩個群組
			if (!result.Any(r => r.Label == "非訂閱會員"))
				result.Add(new { Label = "非訂閱會員", Count = 0 });

			if (!result.Any(r => r.Label == "訂閱會員"))
				result.Add(new { Label = "訂閱會員", Count = 0 });

			return Json(result);
		}

		// 2️⃣ 銅 / 銀 / 金比例
		[HttpGet]
		public IActionResult GetMemberLevelRatio()
		{
			var result = _context.Customers
		.Where(c => c.Level != null && c.Level != 0)
		.Join(_context.CustomerLevels,
			  c => c.Level,
			  l => l.Level,
			  (c, l) => new { l.LevelName })
		.GroupBy(x => x.LevelName)
		.Select(g => new
		{
			Label = g.Key,
			Count = g.Count()
		})
		.ToList();

			return Json(result);

		}

		[HttpGet]
		public IActionResult MemberSubscriptionRatio()
		{
			return View();
		}
	}
}
