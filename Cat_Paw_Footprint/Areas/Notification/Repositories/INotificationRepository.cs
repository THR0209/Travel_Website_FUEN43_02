using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Repositories
{
	/// <summary>
	/// 通知 Repository 介面定義
	/// </summary>
	public interface INotificationRepository
	{
		Task<IEnumerable<Notifications>> GetByCustomerIdAsync(int customerId);
		Task AddAsync(Notifications entity);
		Task MarkAsReadAsync(int id);
		Task<Notifications?> GetByIdAsync(int id);
		Task MarkAllAsReadAsync(int customerId);
	}
}