using Cat_Paw_Footprint.Areas.Employee.Repositories;
using Cat_Paw_Footprint.Areas.Employee.ViewModel;

namespace Cat_Paw_Footprint.Areas.Employee.Services
{
	public class EmployeeService : IEmployeeService
	{
		private readonly IEmployeeRepository _repo;
		public EmployeeService(IEmployeeRepository repo)
		{
			_repo = repo;
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
			throw new NotImplementedException();
		}

		public async Task UpdateAsync(EmployeeViewModel vm)// 更新員工（含 Profile）
		{
			throw new NotImplementedException();
		}
		public async Task UpdateAccountAsync(int id, bool status, string? password, int roleId)
		{
			// 正規化：空白或空字串一律視為不改密碼（傳 null）
			var newPwd = string.IsNullOrWhiteSpace(password) ? null : password;
			await _repo.UpdateStatusAndPasswordAndRoleAsync(id, status, newPwd, roleId);
		}
	}
}
