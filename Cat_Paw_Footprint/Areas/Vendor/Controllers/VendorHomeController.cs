using Cat_Paw_Footprint.Areas.Vendor.Services;
using Cat_Paw_Footprint.Areas.Vendor.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
namespace Cat_Paw_Footprint.Areas.Vendor.Controllers
{
	[Area("Vendor")]
	public class VendorHomeController : Controller
	{
		
		private readonly EmployeeDbContext _context;
		private readonly IVendorHomeService _svc;
		public VendorHomeController(EmployeeDbContext context, IVendorHomeService svc)
		{
			_context = context;
			_svc = svc;
		}
	}
}
