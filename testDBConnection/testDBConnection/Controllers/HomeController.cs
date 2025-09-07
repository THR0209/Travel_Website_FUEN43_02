using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testDBConnection.Models;

namespace testDBConnection.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly MyDbContext _context;

        public HomeController(MyDbContext context)
        {
            _context = context;
		}

		//public HomeController(ILogger<HomeController> logger)
  //      {
  //          _logger = logger;
  //      }

        public string Index()
        {
			//return "Hello World!";
			return _context.Customers.FirstOrDefault()?.Account
		   ?? "No customer found";
		}

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
