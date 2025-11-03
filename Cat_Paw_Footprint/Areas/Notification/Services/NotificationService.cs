using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Repositories;
using Cat_Paw_Footprint.Areas.Notification.Services;

namespace Cat_Paw_Footprint.Services
{
	public class NotificationService : INotificationService
	{
		private readonly INotificationRepository _repo;

		public NotificationService(INotificationRepository repo)
		{
			_repo = repo;
		}

		public async Task<IEnumerable<Notifications>> GetUserNotificationsAsync(int customerId)
		{
			var notifications = await _repo.GetByCustomerIdAsync(customerId);
			return notifications.Select(n => new Notifications
			{
				NotificationID = n.NotificationID,
				Title = n.Title,
				Message = n.Message,
				Type = n.Type,
				IsRead = n.IsRead,
				CreatedAt = n.CreatedAt,
				ReadAt = n.ReadAt
			});
		}

		public async Task AddNotificationAsync(int customerId, string title, string message, string type = "一般")
		{
			var notification = new Notifications
			{
				CustomerID = customerId,
				Title = title,
				Message = message,
				Type = type,
				IsRead = false,
				CreatedAt = DateTime.Now
			};
			await _repo.AddAsync(notification);
		}

		public async Task MarkAsReadAsync(int id)
		{
			await _repo.MarkAsReadAsync(id);
		}

		/// <summary>
		/// 全部標記為已讀
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		public async Task MarkAllAsReadAsync(int customerId)
		{
			await _repo.MarkAllAsReadAsync(customerId);
		}

	}
}
