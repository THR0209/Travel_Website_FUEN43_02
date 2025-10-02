using Cat_Paw_Footprint.Areas.CustomerService.Repositories;
using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// 客戶服務評價服務層，負責評價的查詢與刪除操作
	/// </summary>
	public class CustomerSupportFeedbackService : ICustomerSupportFeedbackService
	{
		private readonly ICustomerSupportFeedbackRepository _repo;

		/// <summary>
		/// 透過 DI 注入評價 Repository
		/// </summary>
		public CustomerSupportFeedbackService(ICustomerSupportFeedbackRepository repo)
		{
			_repo = repo;
		}

		/// <summary>
		/// 取得所有評價
		/// </summary>
		public Task<List<CustomerSupportFeedback>> GetAllAsync() => _repo.GetAllAsync();

		/// <summary>
		/// 依評價 ID 取得單筆評價
		/// </summary>
		public Task<CustomerSupportFeedback?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

		/// <summary>
		/// 刪除評價（依 ID）
		/// </summary>
		public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
	}
}