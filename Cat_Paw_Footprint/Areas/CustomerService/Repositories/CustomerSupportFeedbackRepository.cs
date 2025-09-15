using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	public class CustomerSupportFeedbackRepository : ICustomerSupportFeedbackRepository
	{
		private readonly webtravel2Context _context;

		public CustomerSupportFeedbackRepository(webtravel2Context context)
		{
			_context = context;
		}

		// 取得所有 Feedback (包含工單資料)
		public async Task<List<CustomerSupportFeedback>> GetAllAsync()
		{
			return await _context.CustomerSupportFeedback
				.Include(f => f.Ticket)
				.ToListAsync();
		}

		// 依 ID 取得單筆
		public async Task<CustomerSupportFeedback?> GetByIdAsync(int id)
		{
			return await _context.CustomerSupportFeedback
				.Include(f => f.Ticket)
				.FirstOrDefaultAsync(f => f.FeedbackID == id);
		}

		// 刪除
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
