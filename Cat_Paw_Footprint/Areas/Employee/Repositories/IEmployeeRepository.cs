using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.Employee.Repositories
{
	public interface IEmployeeRepository
	{
		// 取得所有員工（含角色 & 個資）
		Task<IEnumerable<EmployeeViewModel>> GetAllAsync();

		// 依 ID 取得單一員工（含角色 & 個資）
		Task<EmployeeViewModel?> GetByIdAsync(int id);
		// 更新員工（含 Profile）
		Task UpdateAsync(Employees employee, EmployeeProfile? profile = null);
		//由管理員權限者更新員工帳號啟用狀態與權限變換
		
		//由管理員權限者更新員工帳號啟用密碼與狀態與權限變換
		Task UpdateStatusAndPasswordAndRoleAsync(int id, bool Status, string Password, int roleId);

	}
}
