using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 客戶服務工單資料存取層，負責工單 CRUD 及查詢操作
	/// </summary>
	public class CustomerSupportTicketsRepository : ICustomerSupportTicketsRepository
	{
		private readonly webtravel2Context _context;

		/// <summary>
		/// 透過 DI 注入 DbContext
		/// </summary>
		public CustomerSupportTicketsRepository(webtravel2Context context)
		{
			_context = context;
		}

		/// <summary>
		/// 取得所有工單資料，包含關聯的客戶、員工、優先度、狀態、類型
		/// </summary>
		public async Task<IEnumerable<CustomerSupportTickets>> GetAllAsync()
		{
			return await _context.CustomerSupportTickets
				.Include(t => t.Customer)
				.Include(t => t.Employee).ThenInclude(e => e.EmployeeProfile)
				.Include(t => t.Priority)
				.Include(t => t.Status)
				.Include(t => t.TicketType)
				.AsNoTracking()
				.ToListAsync();
		}

		/// <summary>
		/// 依工單 ID 取得單筆工單，包含關聯資料
		/// </summary>
		public async Task<CustomerSupportTickets?> GetByIdAsync(int id)
		{
			return await _context.CustomerSupportTickets
				.Include(t => t.Customer)
				.Include(t => t.Employee).ThenInclude(e => e.EmployeeProfile)
				.Include(t => t.Priority)
				.Include(t => t.Status)
				.Include(t => t.TicketType)
				//.AsNoTracking() // 若欲追蹤物件狀態可註解此行
				.FirstOrDefaultAsync(t => t.TicketID == id);
		}

		/// <summary>
		/// 新增工單
		/// </summary>
		public async Task AddAsync(CustomerSupportTickets ticket)
		{
			await _context.CustomerSupportTickets.AddAsync(ticket);
			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// 更新工單
		/// </summary>
		public async Task UpdateAsync(CustomerSupportTickets ticket)
		{
			_context.CustomerSupportTickets.Update(ticket);
			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// 刪除工單（依 ID）
		/// </summary>
		public async Task DeleteAsync(int id)
		{
			var ticket = await _context.CustomerSupportTickets.FindAsync(id);
			if (ticket != null)
			{
				_context.CustomerSupportTickets.Remove(ticket);
				await _context.SaveChangesAsync();
			}
		}

		/// <summary>
		/// 檢查指定工單是否存在
		/// </summary>
		public async Task<bool> ExistsAsync(int id)
		{
			return await _context.CustomerSupportTickets.AnyAsync(t => t.TicketID == id);
		}
	}
}