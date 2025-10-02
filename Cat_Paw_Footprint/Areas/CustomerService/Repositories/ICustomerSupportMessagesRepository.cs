using Cat_Paw_Footprint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 客戶服務訊息資料存取介面，定義訊息的查詢與新增方法
	/// </summary>
	public interface ICustomerSupportMessagesRepository
	{
		/// <summary>
		/// 取得指定工單的所有訊息
		/// </summary>
		Task<IEnumerable<CustomerSupportMessages>> GetByTicketIdAsync(int ticketId);

		/// <summary>
		/// 取得指定工單的訊息 (分頁)
		/// </summary>
		Task<IEnumerable<CustomerSupportMessages>> GetByTicketIdAsync(int ticketId, int skip, int take);

		/// <summary>
		/// 新增一筆訊息
		/// </summary>
		Task<CustomerSupportMessages> AddAsync(CustomerSupportMessages message);
	}
}