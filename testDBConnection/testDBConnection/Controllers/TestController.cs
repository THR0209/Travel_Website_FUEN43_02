using Microsoft.AspNetCore.Mvc;
using testDBConnection.Models;

namespace testDBConnection.Controllers
{
	public class TestController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		private readonly MyDbContext _context;

		public TestController(MyDbContext context)
		{
			_context = context;
		}

		public IActionResult CheckDb()
		{
			try
			{
				bool canConnect = _context.Database.CanConnect();
				return Content(canConnect ? "✅ 資料庫連線成功！" : "❌ 無法連線資料庫");
			}
			catch (Exception ex)
			{
				return Content("錯誤：" + ex.Message);
			}
		}
	}
}
