using Cat_Paw_Footprint.Areas.Notification.Services;
//using Cat_Paw_Footprint.Areas.Order.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Hubs;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace Cat_Paw_Footprint.Services
{
	public class NotificationTriggerService : INotificationTriggerService
	{
		private readonly INotificationService _notifSvc;
		private readonly webtravel2Context _db;
		private readonly IHubContext<NotificationHub> _hub;
        private readonly IEmailSender _emailSender;

        public NotificationTriggerService(
			INotificationService notifSvc,
			webtravel2Context db,
			IHubContext<NotificationHub> hub, IEmailSender emailSender)
		{
			_notifSvc = notifSvc;
			_db = db;
			_hub = hub;
            _emailSender = emailSender;
        }

		// 🔹 訂單建立
		public async Task NotifyOrderCreatedAsync(int customerId, int orderId)
		{
			await SendAsync(customerId, "訂單成立通知", $"您的訂單 #{orderId} 已成立並完成付款，感謝您的購買！", "系統公告");
		}

		// 🔹 付款成功
		public async Task NotifyPaymentSuccessAsync(int customerId, int orderId)
		{
			await SendAsync(
				customerId,
				"付款成功通知",
				$"您的訂單 #{orderId} 已成功付款，我們將為您準備旅程的詳細資訊，敬請期待！ 🐾",
				"系統公告"
			);
		}

		// 🔹 訂單取消通知
		public async Task NotifyOrderCanceledAsync(int customerId, int orderId)
		{
			var order = await _db.CustomerOrders.FindAsync(orderId);
			string orderCode = order?.CreateTime != null
				? $"ORD-{order.CreateTime:yyyyMMdd}-{order.OrderID}"
				: $"#{orderId}";

			await SendAsync(
				customerId,
				"訂單取消通知",
				$"您的訂單 #{orderId} 已提交取消申請，我們的客服人員將儘快處理。",
				"系統公告"
			);
		}

		// 🔹 客服回覆
		public async Task NotifyCustomerServiceReplyAsync(int customerId, int ticketId)
		{
			await SendAsync(customerId, "客服回覆通知", $"客服人員回覆了您的工單 #{ticketId}", "客服訊息");
		}

		// 🔹 每日簽到提醒
		public async Task NotifyDailySignInAsync(int customerId)
		{
			await SendAsync(customerId, "每日簽到提醒", "別忘了每日簽到領取爪爪幣！", "系統提醒");
		}

        // 🔹 優惠券即將到期
        public async Task NotifyCouponExpiringAsync(int daysBefore = 3)
        {
            var now = DateTime.Now;
            var soon = now.AddDays(daysBefore);

            var expiring = await _db.CustomerCouponsRecords
			 .Include(r => r.Coupon)
			 .Where(r => r.ExpireTime != null &&r.ExpireTime < soon &&
             (r.IsUsed == false || r.IsUsed == null))
			 .ToListAsync();

            foreach (var r in expiring)
            {
                if (r.CustomerID <= 0) continue;

                await SendAsync(
                    r.CustomerID,
                    "優惠券即將到期",
                    $"您的優惠券「{r.Coupon.CouponName}」將於 {r.ExpireTime:MM/dd} 到期，別忘了使用喔！",
                    "優惠活動"
                );
            }
        }

        public async Task NotifyCouponIssuedAsync(int customerId, int couponId)
        {
            var coupon = await _db.Coupons.FindAsync(couponId);
            var customer = await _db.Customers.FindAsync(customerId);
			DateTime expireTime;
            if (coupon == null || customer == null) return;
			if(coupon.ValidDays != null && coupon.ValidDays > 0)
			{
				expireTime = DateTime.Now.AddDays(coupon.ValidDays.Value);
			}
			else
			{
				expireTime = coupon.EndDate;
            }

			string title = "您獲得了一張新的優惠券！";
            string message = $"優惠券「{coupon.CouponName}」已發放至您的帳戶，可使用至 {expireTime:yyyy/MM/dd}";
            await SendCustomAsync(customerId, title, message, "優惠活動");


			////         // ✅ 寄出 Email 通知
			//string htmlMessage = $@"
			//<h2>{title}</h2>
			//<p>{message}</p>
			//<p style='color:gray;font-size:12px;'>此信件由系統自動發送，請勿直接回覆。</p>";

			//var customerEmail = await _db.CustomerProfile
			//.Where(p => p.CustomerID == customerId)
			//.Select(p => p.Email)
			//.FirstOrDefaultAsync();

			//if (!string.IsNullOrWhiteSpace(customerEmail))
			//{
			//	await _emailSender.SendEmailAsync(customerEmail, "貓爪足跡｜新的優惠券通知", htmlMessage);
			//}
			//else
			//{
			//	Console.WriteLine($"找不到客戶 {customerId} 的 Email，跳過寄信。");
			//}
		}


        // 🆕 🔹 客服工單完成通知
        public async Task NotifyTicketCompletedAsync(int ticketId)
		{
			var ticket = await _db.CustomerSupportTickets
				.Include(t => t.Customer)
				.FirstOrDefaultAsync(t => t.TicketID == ticketId);

			if (ticket == null || ticket.CustomerID == null)
				return;

			var customerId = ticket.CustomerID.Value;
			var subject = ticket.Subject ?? "(無主題)";

			await SendAsync(
				customerId,
				"客服服務已完成",
				$"您的客服工單 # {ticket.TicketID} 「{subject}」 已處理完成，請留下服務評價 🐾",
				"客服評價提醒"
			);
		}



		// ------------------------
		// 🧩 共用內部函式
		// ------------------------
		private async Task SendAsync(int? customerId, string title, string message, string type)
		{
			if (customerId == null || customerId <= 0) return;

			await _notifSvc.AddNotificationAsync(customerId.Value, title, message, type);
			await _hub.Clients.User(customerId.Value.ToString())
				.SendAsync("ReceiveNotification", title, message, type);
		}
		public async Task SendCustomAsync(int customerId, string title, string message, string type)
		{
            //Console.WriteLine($"[SendCustomAsync] customerId={customerId}, title={title}");
			if (customerId <= 0) { Console.WriteLine("[SendCustomAsync] 無效的 customerId"); return; }
			
			await _notifSvc.AddNotificationAsync(customerId, title, message, type);
            //Console.WriteLine("[SendCustomAsync] 已呼叫 AddNotificationAsync");
            await _hub.Clients.User(customerId.ToString()).SendAsync("ReceiveNotification", title, message, type);          
		}


	}
}
