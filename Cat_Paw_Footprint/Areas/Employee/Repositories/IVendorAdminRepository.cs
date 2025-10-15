using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Models;
namespace Cat_Paw_Footprint.Areas.Employee.Repositories
{
	public interface IVendorAdminRepository
	{
		Task<IEnumerable<VendorAdminViewModel>> GetAllVendorsAsync();// 查詢所有廠商
		Task<VendorAdminViewModel?> GetVendorByAccountAsync(string account);// 根據帳號查詢廠商詳細資料
		Task<bool> UpdateVendorStatusAsync(string account, bool status);// 更新廠商啟用狀態
		Task<IEnumerable<VendorAdminViewModel>> GetVendorsLoginHistoryAsync(int VendorId);// 查詢廠商登入歷史記錄
	}
}
