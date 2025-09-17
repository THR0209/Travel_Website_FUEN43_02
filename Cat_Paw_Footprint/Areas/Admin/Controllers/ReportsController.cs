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

			// 本月
			var firstDayOfThisMonth = new DateTime(now.Year, now.Month, 1);
			var firstDayOfNextMonth = firstDayOfThisMonth.AddMonths(1);

			// 上月
			var firstDayOfLastMonth = firstDayOfThisMonth.AddMonths(-1);

			// 1. 本月訂單數
			var thisMonthOrders = _context.CustomerOrders
				.Count(o => o.CreateTime >= firstDayOfThisMonth && o.CreateTime < firstDayOfNextMonth);

			var lastMonthOrders = _context.CustomerOrders
				.Count(o => o.CreateTime >= firstDayOfLastMonth && o.CreateTime < firstDayOfThisMonth);

			double orderGrowthRate = lastMonthOrders > 0
				? ((double)(thisMonthOrders - lastMonthOrders) / lastMonthOrders) * 100
				: 100;

			// 2. 開放行程數 (Products.IsActive) → 不需要比較，因為是即時狀態
			var openTrips = _context.Products
				.Count(p => p.IsActive == true);

			// 3. 客戶總數
			var customerCount = _context.Customers.Count();
			var lastMonthCustomers = _context.Customers.Count(c => c.CreateDate < firstDayOfThisMonth);

			double customerGrowthRate = lastMonthCustomers > 0
				? ((double)(customerCount - lastMonthCustomers) / lastMonthCustomers) * 100
				: 100;

			// 4. 已解決案件數
			var resolvedTickets = _context.CustomerSupportTickets
				.Count(t => t.StatusID == 3);

			var lastMonthResolved = _context.CustomerSupportTickets
				.Count(t => t.StatusID == 3 && t.UpdateTime < firstDayOfThisMonth);

			double ticketGrowthRate = lastMonthResolved > 0
				? ((double)(resolvedTickets - lastMonthResolved) / lastMonthResolved) * 100
				: 100;

			var vm = new DashboardViewModel
			{
				MonthlyOrders = thisMonthOrders,
				OpenTrips = openTrips,
				CustomerCount = customerCount,
				ResolvedTickets = resolvedTickets,
				OrderGrowthRate = orderGrowthRate,
				CustomerGrowthRate = customerGrowthRate,
				TicketGrowthRate = ticketGrowthRate
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
        public IActionResult MemberSubscriptionRatio()
        {
            //查資料庫
            var data = new
            {
                Subscribed = 350,
                NotSubscribed = 150
            };
            return View(data);
        }

    }
}
