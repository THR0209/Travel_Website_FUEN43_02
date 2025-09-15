using Cat_Paw_Footprint.Areas.Employee.Repositories;
using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.Employee.Services
{
	public class EmployeeService : IEmployeeService
	{
		private readonly IEmployeeRepository _repo;
		private readonly EmployeeDbContext _context;
		public EmployeeService(IEmployeeRepository repo, EmployeeDbContext context)
		{
			_repo = repo;
			_context = context;
		}
		public async Task<IEnumerable<EmployeeViewModel>> GetAllAsync()// 取得所有員工（含角色 & 個資）
		{
			// 直接呼叫 Repository
			var employees = await _repo.GetAllAsync();

			// ⚡ 這裡可以加邏輯，例如過濾只顯示啟用中的員工
			return employees;
		}

		public async Task<EmployeeViewModel?> GetByIdAsync(int id)// 依 ID 取得單一員工（含角色 & 個資）
		{
			return await _repo.GetByIdAsync(id);
		}

		public async Task<bool> UpdateSelfAsync(int empId, string name, string? phone, string? email, string? address, byte[]? photo, string? newPassword, string? idNumber)// 更新員工
		{
			var newPasswordHash = string.IsNullOrWhiteSpace(newPassword) ? null : BCrypt.Net.BCrypt.HashPassword(newPassword);
			var newpjoto = photo == null ? null : photo;

			return await _repo.UpdateSelfAsync(empId, name, phone, email, address, photo, newPasswordHash, idNumber);
		}
		public async Task UpdateAccountAsync(int id, bool status, string? password, int roleId)
		{
			// 正規化：空白或空字串一律視為不改密碼（傳 null）
			var newPwd = string.IsNullOrWhiteSpace(password) ? null : password;
			await _repo.UpdateStatusAndPasswordAndRoleAsync(id, status, newPwd, roleId);
		}
		public async Task<string> GetNewEmployeeCodeAsync()
		{
			var outputParam = new SqlParameter
			{
				ParameterName = "@NewSerial",
				SqlDbType = System.Data.SqlDbType.VarChar,
				Size = 20,
				Direction = System.Data.ParameterDirection.Output
			};

			await _context.Database.ExecuteSqlRawAsync(
				"EXEC GetNewSerialNumberNoDate @SourceTable, @Prefix, @NewSerial OUTPUT",
				new SqlParameter("@SourceTable", "Employees"),
				new SqlParameter("@Prefix", "EMP"),
				outputParam
			);
			return outputParam.Value?.ToString() ?? string.Empty;
		}
	}
}
