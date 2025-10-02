using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 客戶服務訊息資料存取層，負責訊息查詢與新增
	/// </summary>
	public class CustomerSupportMessagesRepository : ICustomerSupportMessagesRepository
	{
		private readonly webtravel2Context _context;

		/// <summary>
		/// 透過 DI 注入 DbContext
		/// </summary>
		public CustomerSupportMessagesRepository(webtravel2Context context) => _context = context;

		/// <summary>
		/// 取得指定工單的所有訊息，依送出時間排序
		/// </summary>
		public async Task<IEnumerable<CustomerSupportMessages>> GetByTicketIdAsync(int ticketId)
		{
			return await _context.CustomerSupportMessages
				.Where(m => m.TicketID == ticketId)
				.OrderBy(m => m.SentTime)
				.ToListAsync();
		}

		/// <summary>
		/// 取得指定工單的訊息 (分頁)，依送出時間排序
		/// </summary>
		public async Task<IEnumerable<CustomerSupportMessages>> GetByTicketIdAsync(int ticketId, int skip, int take)
		{
			return await _context.CustomerSupportMessages
			   .Where(m => m.TicketID == ticketId)
			   .OrderBy(m => m.SentTime)
			   .Skip(skip)
			   .Take(take)
			   .ToListAsync();
		}

		/// <summary>
		/// 新增一筆訊息
		/// </summary>
		public async Task<CustomerSupportMessages> AddAsync(CustomerSupportMessages message)
		{
			_context.CustomerSupportMessages.Add(message);
			await _context.SaveChangesAsync();
			return message;
		}
	}
}