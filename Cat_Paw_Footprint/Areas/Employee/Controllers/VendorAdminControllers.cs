using Cat_Paw_Footprint.Areas.Employee.Services;
using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;

namespace Cat_Paw_Footprint.Areas.Employee.Controllers
{
	[Area("Employee")]
	public class VendorAdminControllers
	{
		private readonly EmployeeDbContext _context;
		private readonly IVendorAdminService _svc;

		public VendorAdminControllers(EmployeeDbContext context, IVendorAdminService svc)
		{
			_context = context;
			_svc = svc;
		}

	}
}
