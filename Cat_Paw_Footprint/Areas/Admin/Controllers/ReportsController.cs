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
			return View();
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
