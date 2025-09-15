using Cat_Paw_Footprint.Areas.Employee.ViewModel;
namespace Cat_Paw_Footprint.Areas.Employee.Services
{
	public interface IVendorAdminService
	{
		Task<IEnumerable<VendorAdminViewModel>> GetAllVendorsAsync();// 查詢所有廠商
		Task<VendorAdminViewModel?> GetVendorByAccountAsync(string account);// 根據帳號查詢廠商詳細資料
		Task<bool> UpdateStatusAsync(string account, bool status);// 更新廠商啟用狀態
		Task<IEnumerable<VendorAdminViewModel>> GetVendorsLoginHistoryAsync(int VendorId);// 查詢廠商登入歷史記錄
		Task<bool> RegisterVendorAsync(VendorAdminViewModel model);// 廠商註冊
	}
}
