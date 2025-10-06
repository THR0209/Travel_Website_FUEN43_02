using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
namespace Cat_Paw_Footprint.Areas.CustomersArea.Services
{
	public interface ICusLogRegService//登入註冊修改資料
	{
		Task<string?> RegisterCustomerAsync(CusLogRegDto model);// 客戶註冊
		Task<CusLogRegDto?> UpdateCustomerAsync(CusLogRegDto model);// 客戶修改資料
		Task<CusLogRegDto?> LoginAsync(string account, string password, string ip);// 客戶登入
		Task<CusLogRegDto?> GetCustomerByAccountAsync(string Account);// 根據帳號查詢客戶

	}
}
