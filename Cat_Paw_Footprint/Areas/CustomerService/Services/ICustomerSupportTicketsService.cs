using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// 客戶服務工單服務介面，定義工單的 CRUD 及查詢方法（共用於前台與後台）
	/// </summary>
	public interface ICustomerSupportTicketsService
	{
		/// <summary>
		/// 取得所有工單（ViewModel）
		/// </summary>
		Task<IEnumerable<CustomerSupportTicketViewModel>> GetAllAsync();

		/// <summary>
		/// 依工單 ID 取得單筆工單（ViewModel）
		/// </summary>
		Task<CustomerSupportTicketViewModel?> GetByIdAsync(int id);

		/// <summary>
		/// 新增工單（ViewModel）
		/// </summary>
		Task AddAsync(CustomerSupportTicketViewModel vm);

		/// <summary>
		/// 更新工單（ViewModel）
		/// </summary>
		Task UpdateAsync(CustomerSupportTicketViewModel vm);

		/// <summary>
		/// 刪除工單（依 ID）
		/// </summary>
		Task DeleteAsync(int id);

		/// <summary>
		/// 檢查指定工單是否存在
		/// </summary>
		Task<bool> ExistsAsync(int id);

	}
}