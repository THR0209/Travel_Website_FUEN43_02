using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Repositories
{
	/// <summary>
	/// 通知資料存取層（Repository）
	/// 負責與資料庫進行直接互動
	/// </summary>
	public class NotificationRepository : INotificationRepository
	{
		private readonly webtravel2Context _context;

		public NotificationRepository(webtravel2Context context)
		{
			_context = context;
		}

		/// <summary>
		/// 取得某會員的所有通知（依建立時間由新到舊）
		/// </summary>
		public async Task<IEnumerable<Notifications>> GetByCustomerIdAsync(int customerId)
		{
			return await _context.Notifications
				.Where(n => n.CustomerID == customerId)
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();
		}

		/// <summary>
		/// 新增通知
		/// </summary>
		public async Task AddAsync(Notifications entity)
		{
			_context.Notifications.Add(entity);
			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// 將通知標記為已讀
		/// </summary>
		public async Task MarkAsReadAsync(int id)
		{
			var n = await _context.Notifications.FindAsync(id);
			if (n != null)
			{
				n.IsRead = true;
				n.ReadAt = DateTime.Now;
				await _context.SaveChangesAsync();
			}
		}

		/// <summary>
		/// 查詢特定通知
		/// </summary>
		public async Task<Notifications?> GetByIdAsync(int id)
		{
			return await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationID == id);
		}

		/// <summary>
		/// 將該會員的所有通知標記為已讀
		/// </summary>
		public async Task MarkAllAsReadAsync(int customerId)
		{
			var list = await _context.Notifications
				.Where(n => n.CustomerID == customerId && !n.IsRead)
				.ToListAsync();

			if (list.Any())
			{
				foreach (var n in list)
				{
					n.IsRead = true;
					n.ReadAt = DateTime.Now;
				}
				await _context.SaveChangesAsync();
			}
		}

	}

}
