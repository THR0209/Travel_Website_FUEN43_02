using Cat_Paw_Footprint.Areas.Admin.ViewModel;
using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaAdmin")]
	public class ReportsController : Controller
    {
        private readonly webtravel2Context _context;

        public ReportsController(webtravel2Context context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
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

        // 3️⃣ 銷售報表（日/月/季/年）
        [HttpGet]
        public IActionResult GetSalesReport(string groupBy = "month")
        {
            var query = _context.CustomerOrders
                .Where(o => o.CreateTime.HasValue)
                .AsQueryable();

            List<object> result;

            switch (groupBy.ToLower())
            {
                case "day":
                    var dailyData = query
                        .GroupBy(o => o.CreateTime.Value.Date)
                        .AsEnumerable()
                        .Select(g => new
                        {
                            x = g.Key,
                            y = g.Sum(o => o.TotalAmount ?? 0)
                        })
                        .OrderBy(r => r.x)
                        .ToList();

                    // 取最近 7 天
                    var today = DateTime.Today;
                    var last7Days = Enumerable.Range(0, 7)
                        .Select(i => today.AddDays(-6 + i));

                    result = last7Days
                        .GroupJoin(
                            dailyData,
                            d => d,
                            g => g.x,
                            (d, g) => new
                            {
                                x = d.ToString("yyyy-MM-dd"),
                                y = g.Sum(x => x.y)
                            }
                        )
                        .ToList<object>();
                    break;

                case "month":
                    var monthlyData = query
                        .GroupBy(o => new { o.CreateTime.Value.Year, o.CreateTime.Value.Month })
                        .AsEnumerable()
                        .Select(g => new
                        {
                            x = new DateTime(g.Key.Year, g.Key.Month, 1),
                            y = g.Sum(o => o.TotalAmount ?? 0)
                        })
                        .OrderBy(r => r.x)
                        .ToList();

                    // 最近 12 個月
                    var startMonth = DateTime.Today.AddMonths(-11);
                    var allMonths = Enumerable.Range(0, 12)
                        .Select(i => new DateTime(startMonth.Year, startMonth.Month, 1).AddMonths(i));

                    result = allMonths
                        .GroupJoin(
                            monthlyData,
                            m => new { m.Year, m.Month },
                            d => new { d.x.Year, d.x.Month },
                            (m, g) => new
                            {
                                x = m.ToString("yyyy-MM"),
                                y = g.Sum(x => x.y)
                            }
                        )
                        .ToList<object>();
                    break;

                case "quarter":
                    var quarterlyData = query
                        .GroupBy(o => new { o.CreateTime.Value.Year, Quarter = (o.CreateTime.Value.Month - 1) / 3 + 1 })
                        .AsEnumerable()
                        .Select(g => new
                        {
                            x = new { g.Key.Year, g.Key.Quarter },
                            y = g.Sum(o => o.TotalAmount ?? 0)
                        })
                        .OrderBy(r => r.x.Year).ThenBy(r => r.x.Quarter)
                        .ToList();

                    // 最近 4 季
                    var now = DateTime.Today;
                    var currentQuarter = (now.Month - 1) / 3 + 1;
                    var currentYear = now.Year;

                    // 建立最近 4 季的清單
                    var quarters = new List<(int Year, int Quarter)>();
                    for (int i = 3; i >= 0; i--) // 取當季 + 前 3 季
                    {
                        int year = currentYear;
                        int quarter = currentQuarter - i;
                        if (quarter <= 0)
                        {
                            quarter += 4;
                            year -= 1;
                        }
                        quarters.Add((year, quarter));
                    }

                    result = quarters
                        .GroupJoin(
                            quarterlyData,
                            q => new { q.Year, q.Quarter },
                            d => new { d.x.Year, d.x.Quarter },
                            (q, g) => new
                            {
                                x = $"{q.Year} Q{q.Quarter}",
                                y = g.Sum(x => x.y)
                            }
                        )
                        .OrderBy(r => r.x) // 按時間排序
                        .ToList<object>();
                    break;


                case "year":
                    var yearlyData = query
                        .GroupBy(o => o.CreateTime.Value.Year)
                        .AsEnumerable()
                        .Select(g => new
                        {
                            x = g.Key,
                            y = g.Sum(o => o.TotalAmount ?? 0)
                        })
                        .OrderBy(r => r.x)
                        .ToList();

                    result = yearlyData
                        .Select(d => new
                        {
                            x = d.x.ToString(),
                            y = d.y
                        })
                        .ToList<object>();
                    break;

                default:
                    return BadRequest("Invalid groupBy value. Use day, month, quarter, or year.");
            }

            return Json(result);
        }
    }
}

