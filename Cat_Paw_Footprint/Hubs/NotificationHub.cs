using Microsoft.AspNetCore.SignalR;

namespace Cat_Paw_Footprint.Hubs
{
	public class NotificationHub : Hub
	{
		// 發送給指定使用者
		public async Task SendNotificationToUser(string userId, string title, string message, string type = "一般")
		{
			await Clients.User(userId).SendAsync("ReceiveNotification", title, message, type);
		}
	}
}
