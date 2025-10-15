using Cat_Paw_Footprint.Areas.Employee.Repositories;
using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Models;
namespace Cat_Paw_Footprint.Areas.Employee.Services
{
	public interface ICustomerAdminService
	{
		// 查詢客戶帳號
		Task<CustomerAdminViewModel?> GetCustomerByIdAsync(int? customerId);
		Task<IEnumerable<CustomerAdminViewModel>> GetAllCustomersAsync();

		// 查詢登入紀錄
		Task<IEnumerable<CustomerAdminViewModel?>> GetLoginHistoryAsync(int? customerId);

		// 修改客戶資料
		Task<bool> UpdateLevelAsync(int customerId, int levelId);
		Task<bool> UpdateBlacklistStatusAsync(int customerId, bool isBlacklisted);
		Task<bool> UpdateStatusAsync(int customerId, bool status);

		// 查詢等級清單
		Task<IEnumerable<CustomerAdminViewModel>> GetAllLevelsAsync();
		//根據信箱查詢id
		Task<int?> GetCustomerIdByEmailAsync(string email);
		Task BatchSaveCustomersAsync(List<CustomerUpdateDto> updates);//批次更新
	}
}
