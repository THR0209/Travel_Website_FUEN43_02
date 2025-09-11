using Cat_Paw_Footprint.Areas.Vendor.ViewModel;
using Cat_Paw_Footprint.Data;     // EmployeeDbContext
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;


namespace Cat_Paw_Footprint.Areas.Vendor.Repositories
{
	public class VendorHomeRepository: IVendorHomeRepository
	{
		private readonly EmployeeDbContext _db;
		public VendorHomeRepository(EmployeeDbContext db)
		{
			_db = db;
		}
	}
}
