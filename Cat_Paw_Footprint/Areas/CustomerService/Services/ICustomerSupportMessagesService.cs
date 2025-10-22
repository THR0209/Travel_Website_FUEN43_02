using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Cat_Paw_Footprint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// 客戶服務訊息服務介面，定義訊息的查詢與新增方法（共用於前台與後台）
	/// </summary>
	public interface ICustomerSupportMessagesService
	{
		/// <summary>
		/// 取得指定工單的所有訊息（ViewModel）
		/// </summary>
		Task<IEnumerable<CustomerSupportMessageViewModel>> GetByTicketIdAsync(int ticketId);

		/// <summary>
		/// 新增一筆訊息（ViewModel）
		/// </summary>
		Task<CustomerSupportMessageViewModel> AddAsync(CustomerSupportMessageViewModel vm);

		/// <summary>
		/// 取得指定工單的訊息 (分頁)（ViewModel）
		/// </summary>
		Task<IEnumerable<CustomerSupportMessageViewModel>> GetByTicketIdAsync(int ticketId, int skip, int take);
	}
}