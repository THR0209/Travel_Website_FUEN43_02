using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 客戶服務評價資料存取介面，定義評價的查詢與刪除方法
	/// </summary>
	public interface ICustomerSupportFeedbackRepository
	{
		/// <summary>
		/// 取得所有評價
		/// </summary>
		Task<List<CustomerSupportFeedback>> GetAllAsync();

		/// <summary>
		/// 依評價 ID 取得單筆評價
		/// </summary>
		Task<CustomerSupportFeedback?> GetByIdAsync(int id);

		/// <summary>
		/// 刪除評價（依 ID）
		/// </summary>
		Task DeleteAsync(int id);
	}
}