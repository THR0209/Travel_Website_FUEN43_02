using Microsoft.AspNetCore.Mvc;

namespace Cat_Paw_Footprint.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ReportsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
