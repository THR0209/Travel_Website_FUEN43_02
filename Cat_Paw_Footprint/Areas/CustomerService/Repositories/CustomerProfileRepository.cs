using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 客戶資料存取層，負責取得所有客戶資料
	/// </summary>
	public class CustomerProfileRepository : ICustomerProfileRepository
	{
		private readonly webtravel2Context _context;

		/// <summary>
		/// 透過 DI 注入 DbContext
		/// </summary>
		public CustomerProfileRepository(webtravel2Context context) => _context = context;

		/// <summary>
		/// 取得所有客戶資料
		/// </summary>
		public async Task<IEnumerable<CustomerProfile>> GetAllAsync()
			=> await _context.CustomerProfile.ToListAsync();
	}
}