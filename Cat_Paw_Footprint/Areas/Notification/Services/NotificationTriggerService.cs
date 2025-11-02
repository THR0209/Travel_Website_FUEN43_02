using Cat_Paw_Footprint.Areas.Notification.Services;
using Cat_Paw_Footprint.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Cat_Paw_Footprint.Hubs;


namespace Cat_Paw_Footprint.Services
{
	public class NotificationTriggerService : INotificationTriggerService
	{
		private readonly INotificationService _notifSvc;
		private readonly webtravel2Context _db;
		private readonly IHubContext<NotificationHub> _hub;

		public NotificationTriggerService(
			INotificationService notifSvc,
			webtravel2Context db,
			IHubContext<NotificationHub> hub
		)
		{
			_notifSvc = notifSvc;
			_db = db;
			_hub = hub;
		}

		public async Task NotifyOrderCreatedAsync(int customerId, int orderId)
		{
			await SendAsync(customerId, "訂單成立通知", $"您的訂單 #{orderId} 已成立並完成付款，感謝您的購買！", "訂單通知");
		}

		public async Task NotifyPaymentSuccessAsync(int customerId, int orderId)
		{
			await SendAsync(
				customerId,
				"付款成功通知",
				$"您的訂單 #{orderId} 已成功付款，我們將為您準備旅程的詳細資訊，敬請期待！ 🐾",
				"訂單通知"
			);
		}

		public async Task NotifyCustomerServiceReplyAsync(int customerId, int ticketId)
		{
			await SendAsync(customerId,"客服回覆通知",$"客服人員回覆了您的工單 #{ticketId}","客服訊息");
		}

		public async Task NotifyDailySignInAsync(int customerId)
		{
			await SendAsync(customerId, "每日簽到提醒", "別忘了每日簽到領取爪爪幣！", "系統提醒");
		}

		public async Task NotifyCouponExpiringAsync(int daysBefore = 3)
		{
			var now = DateTime.Now;
			var soon = now.AddDays(daysBefore);

			var expiring = await _db.CustomerCouponsRecords
				.Include(r => r.Coupon)
				.Where(r => r.Coupon != null &&
							r.Coupon.EndDate != null &&
							r.Coupon.EndDate < soon &&
							(r.IsUsed == false || r.IsUsed == null))
				.ToListAsync();

            foreach (var r in expiring)
            {
                if (r.CustomerID > 0)
                {
                    await SendAsync(
                        r.CustomerID,
                        "優惠券即將到期",
                        $"您的優惠券「{r.Coupon.CouponDesc}」將於 {r.Coupon.EndDate:MM/dd} 到期。",
                        "優惠券"
                    );
                }
            }

        }

        private async Task SendAsync(int? customerId, string title, string message, string type)
		{
			if (customerId == null || customerId <= 0) return;

			await _notifSvc.AddNotificationAsync(customerId.Value, title, message, type);
			await _hub.Clients.User(customerId.Value.ToString())
				.SendAsync("ReceiveNotification", title, message, type);
		}

		public async Task SendCustomAsync(int customerId, string title, string message, string type)
		{
			if (customerId <= 0) return;

			await _notifSvc.AddNotificationAsync(customerId, title, message, type);
			await _hub.Clients.User(customerId.ToString())
				.SendAsync("ReceiveNotification", title, message, type);
		}


	}
}
