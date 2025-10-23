using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[AllowAnonymous]
	public class TravelController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
