using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 客戶資料存取介面，定義取得所有客戶的方法
	/// </summary>
	public interface ICustomerProfileRepository
	{
		/// <summary>
		/// 取得所有客戶資料
		/// </summary>
		Task<IEnumerable<CustomerProfile>> GetAllAsync();
	}
}