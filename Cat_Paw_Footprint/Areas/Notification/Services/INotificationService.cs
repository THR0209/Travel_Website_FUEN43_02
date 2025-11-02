using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.Notification.Services
{
	/// <summary>
	/// 通知服務介面，方便注入與測試
	/// </summary>
	public interface INotificationService
	{
		Task<IEnumerable<Notifications>> GetUserNotificationsAsync(int customerId);
		Task MarkAsReadAsync(int id);
		Task AddNotificationAsync(int customerId, string title, string message, string type = "一般");
		Task MarkAllAsReadAsync(int customerId);
	}
}
