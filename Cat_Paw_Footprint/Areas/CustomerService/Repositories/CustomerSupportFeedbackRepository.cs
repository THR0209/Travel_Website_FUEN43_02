using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 客戶服務評價資料存取層，負責評價查詢與刪除操作
	/// </summary>
	public class CustomerSupportFeedbackRepository : ICustomerSupportFeedbackRepository
	{
		private readonly webtravel2Context _context;

		/// <summary>
		/// 透過 DI 注入 DbContext
		/// </summary>
		public CustomerSupportFeedbackRepository(webtravel2Context context)
		{
			_context = context;
		}

		/// <summary>
		/// 取得所有評價，包含工單資料
		/// </summary>
		public async Task<List<CustomerSupportFeedback>> GetAllAsync()
		{
			return await _context.CustomerSupportFeedback
				.Include(f => f.Ticket)
				.ToListAsync();
		}

		/// <summary>
		/// 依評價 ID 取得單筆評價，包含工單資料
		/// </summary>
		public async Task<CustomerSupportFeedback?> GetByIdAsync(int id)
		{
			return await _context.CustomerSupportFeedback
				.Include(f => f.Ticket)
				.FirstOrDefaultAsync(f => f.FeedbackID == id);
		}

		/// <summary>
		/// 刪除評價（依 ID）
		/// </summary>
		public async Task DeleteAsync(int id)
		{
			var entity = await _context.CustomerSupportFeedback.FindAsync(id);
			if (entity != null)
			{
				_context.CustomerSupportFeedback.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}
	}
}