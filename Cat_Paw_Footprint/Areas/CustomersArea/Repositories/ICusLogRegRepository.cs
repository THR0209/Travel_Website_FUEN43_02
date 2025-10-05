using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Models;


namespace Cat_Paw_Footprint.Areas.CustomersArea.Repositories
{
	public interface ICusLogRegRepository
	{
		Task<CusLogRegDto?> GetCustomerByAccountAsync(string Account);// 根據帳號查詢客戶
		Task<int?> CreateLoginLogAsync(int customerId, bool isSuccessful, string ip);// 建立客戶登入紀錄
		Task<IEnumerable<CusLogRegDto?>> GetLastLoginHistoryAsync(int? customerId);// 查詢客戶最後登入紀錄
	}
}
