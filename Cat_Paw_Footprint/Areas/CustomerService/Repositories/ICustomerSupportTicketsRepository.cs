using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 客戶服務工單資料存取介面，定義工單的 CRUD 及查詢方法
	/// </summary>
	public interface ICustomerSupportTicketsRepository
	{
		/// <summary>
		/// 取得所有工單
		/// </summary>
		Task<IEnumerable<CustomerSupportTickets>> GetAllAsync();

		/// <summary>
		/// 依工單 ID 取得單筆工單
		/// </summary>
		Task<CustomerSupportTickets?> GetByIdAsync(int id);

		/// <summary>
		/// 新增工單
		/// </summary>
		Task AddAsync(CustomerSupportTickets ticket);

		/// <summary>
		/// 更新工單
		/// </summary>
		Task UpdateAsync(CustomerSupportTickets ticket);

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