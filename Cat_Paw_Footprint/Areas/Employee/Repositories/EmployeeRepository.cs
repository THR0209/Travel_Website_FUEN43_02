using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;     // EmployeeDbContext
using Cat_Paw_Footprint.Models;  // Employees / EmployeeRoles / EmployeeProfile
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.Employee.Repositories
{
	public class EmployeeRepository : IEmployeeRepository
	{
		private readonly EmployeeDbContext _db;

		public EmployeeRepository(EmployeeDbContext db)
		{
			_db = db;
		}
		

		public async Task<IEnumerable<EmployeeViewModel>> GetAllAsync()// 取得所有員工（含角色 & 個資）
		{
			return await _db.Employees
			.Select(e => new EmployeeViewModel 
			{
			EmployeeID = e.EmployeeID,//員工編號
				EmployeeName = e.EmployeeProfile.EmployeeName,//員工姓名
				Account = e.Account,//帳號
				RoleID = e.RoleID,//角色代號
				RoleName = e.Role.RoleName,//角色名稱
				CreateDate = e.CreateDate,//建立日期
				Status = e.Status,//帳號啟用狀態
				EmployeeCode = e.EmployeeCode,//員工代號
				EmployeeProfileCode = e.EmployeeProfile.EmployeeProfileCode,//員工資料代號
				IDNumber = e.EmployeeProfile.IDNumber,//身分證字號
				Phone = e.EmployeeProfile.Phone,//電話
				Email = e.EmployeeProfile.Email,//電子郵件
				Address = e.EmployeeProfile.Address,//地址
				Photo = e.EmployeeProfile.Photo//照片
			}).ToListAsync();// 將查詢結果轉換為清單並回傳

		}
		

		public Task<EmployeeViewModel?> GetByIdAsync(int id)// 依 ID 取得單一員工（含角色 & 個資）
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(Employees employee, EmployeeProfile? profile = null)// 更新員工（含 Profile）
		{
			throw new NotImplementedException();
		}

		public async Task UpdateStatusAndPasswordAndRoleAsync(int id, bool status, string? password, int roleId)
		{
			if (!string.IsNullOrWhiteSpace(password))
			{
				var hashed = BCrypt.Net.BCrypt.HashPassword(password);

				await _db.Employees
					.Where(e => e.EmployeeID == id)
					.ExecuteUpdateAsync(s => s
						.SetProperty(e => e.Status, status)
						.SetProperty(e => e.Password, hashed)
						.SetProperty(e => e.RoleID, roleId)
					);
			}
			else
			{
				await _db.Employees
					.Where(e => e.EmployeeID == id)
					.ExecuteUpdateAsync(s => s
						.SetProperty(e => e.Status, status)
						.SetProperty(e => e.RoleID, roleId)
					);
			}
		}
	}
}
