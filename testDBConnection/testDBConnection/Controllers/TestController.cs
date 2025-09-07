using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using testDBConnection.Models;

namespace testDBConnection.Controllers
{
	public class TestController : Controller
	{
		private readonly HttpClient _httpClient;
		

		private readonly MyDbContext _context;

		public IActionResult Index()
		{
			return View();
		}

		public TestController(MyDbContext context, HttpClient httpClient)
		{
			_context = context;
			_httpClient = httpClient;
		}

		public async Task<IActionResult> CheckDb()
		{
			var ip = await _httpClient.GetStringAsync("https://api.ipify.org");
			ViewBag.IpAddress = ip;
			bool canConnect = _context.Database.CanConnect();
			string canConnectString = canConnect ? "✅ 資料庫連線成功！" : "❌ 無法連線資料庫";
			ViewBag.DbConnectionResult = canConnectString;

			var customers= _context.Customers.ToList();

			return View(customers);
				
		}
	}
}
