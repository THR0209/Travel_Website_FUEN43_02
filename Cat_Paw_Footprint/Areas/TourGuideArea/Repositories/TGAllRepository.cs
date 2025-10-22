using Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel;
using Cat_Paw_Footprint.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.TourGuideArea.Repositories
{
	public class TGAllRepository : ITGAllRepository
	{
		private readonly EmployeeDbContext _db;
		public TGAllRepository(EmployeeDbContext db)
		{
			_db = db;
		}
		public async Task<TGLoginDto?> GetGuideByAccountAsync(string account)// 根據帳號查詢導遊
		{
			return await _db.Employees
			.AsNoTracking()
			.Where(e => e.Account == account)
			.Select(e => new TGLoginDto
			{
				EmployeeID = e.EmployeeID,
				EmployeeName = e.EmployeeProfile.EmployeeName,
				RoleID = e.RoleID,
				RoleName = e.Role.RoleName,
				CreateDate = e.CreateDate,
				Status = e.Status,
				EmployeeCode = e.EmployeeCode,
				EmployeeProfileCode = e.EmployeeProfile.EmployeeProfileCode,
				IDNumber = e.EmployeeProfile.IDNumber,
				Phone = e.EmployeeProfile.Phone,
				Email = e.EmployeeProfile.Email,
				Address = e.EmployeeProfile.Address,
				Photo = e.EmployeeProfile.Photo
			})
			.FirstOrDefaultAsync();
		}
		
		
	}
}
