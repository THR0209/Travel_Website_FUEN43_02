using Cat_Paw_Footprint.Areas.Vendor.Repositories;
using Cat_Paw_Footprint.Areas.Vendor.ViewModel;
using Cat_Paw_Footprint.Models;
namespace Cat_Paw_Footprint.Areas.Vendor.Services
{
	public interface IVendorHomeService
	{
		public Task<VendorHomeViewModel> LoginAsync(string account, string password, string ip);//廠商登入
	}
}
