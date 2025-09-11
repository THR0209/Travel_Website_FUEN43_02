using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;     // EmployeeDbContext
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.Employee.Repositories
{
	public class VendorAdminRepository : IVendorAdminRepository
	{
		private readonly EmployeeDbContext _db;
		public VendorAdminRepository(EmployeeDbContext db)
		{
			_db = db;
		}
	}
}
