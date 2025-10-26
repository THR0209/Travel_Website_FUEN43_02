using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
namespace Cat_Paw_Footprint.Areas.CustomersArea.Services
{
	public interface ICusLogRegService//登入註冊修改資料
	{
		Task<string?> RegisterCustomerAsync(CusLogRegDto model);// 客戶註冊
		Task<CusLogRegDto?> UpdateCustomerAsync(CusLogRegDto model);// 客戶修改資料
		Task<CusLogRegDto?> LoginAsync(string account, string password, string ip);// 客戶登入
		Task<CusLogRegDto?> GetCustomerByAccountAsync(string Account);// 根據帳號查詢客戶
		Task<CusLogRegDto?> UpdateCustomerPasswordAsync(string Email, string newPassword);// 客戶修改密碼(同時作用於找回密碼)
		Task<CusLogRegDto?> EmailToUser(string Email);// 發送電子郵件給使用者(找回密碼)
	}
}
