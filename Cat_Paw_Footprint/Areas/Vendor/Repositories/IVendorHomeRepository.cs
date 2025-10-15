using Cat_Paw_Footprint.Areas.Vendor.ViewModel;
using Cat_Paw_Footprint.Models;
namespace Cat_Paw_Footprint.Areas.Vendor.Repositories
{
	public interface IVendorHomeRepository
	{
		Task<VendorHomeViewModel?> GetVendorByAccountAsync(string account);//根據帳號查詢廠商
		Task<int?> CreateLoginLogAsync(int vendorId, bool isSuccessful, string ip);//建立廠商登入紀錄
		Task<IEnumerable<VendorHomeViewModel?>> GetLast3LoginLogsAsync(int vendorId);//查詢廠商前3次登入狀況
		Task<bool?> LockVendorAccountAsync(int vendorId);//連續登入失敗滿3次後廠商鎖帳
		Task<bool?> UpdateLastLoginAsync(int vendorId, DateTime lastLogin, string ip);//更新最後登入時間與IP
	}
}
