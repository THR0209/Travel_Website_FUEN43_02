using Microsoft.AspNetCore.Mvc;
using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("Customers")]
	[Authorize(AuthenticationSchemes = "CustomerAuth")]
	public class CusLogRegController : Controller
	{
		private readonly EmployeeDbContext _context;

		public CusLogRegController(EmployeeDbContext context)
		{
			_context = context;
		}
		//客戶首頁
		public IActionResult Index()
		{
			return View();
		}
	}
}
