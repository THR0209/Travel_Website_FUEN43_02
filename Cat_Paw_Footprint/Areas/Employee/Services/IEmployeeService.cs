using Cat_Paw_Footprint.Areas.Employee.ViewModel;

namespace Cat_Paw_Footprint.Areas.Employee.Services
{
	public interface IEmployeeService
	{
		// 取得所有員工（含角色 & 個資）
		Task<IEnumerable<EmployeeViewModel>> GetAllAsync();

		// 依 ID 取得單一員工（含角色 & 個資）
		Task<EmployeeViewModel?> GetByIdAsync(int id);

		// 更新員工（含 Profile）
		Task<bool> UpdateSelfAsync(int empId, string name, string? phone, string? email, string? address, byte[]? photo, string? newPassword, string? idNumber);
		// 更新員工帳號狀態、密碼、角色
		Task UpdateAccountAsync(int id, bool status, string? password, int roleId);
	}
}
