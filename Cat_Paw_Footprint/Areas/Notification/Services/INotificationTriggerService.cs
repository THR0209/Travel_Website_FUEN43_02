namespace Cat_Paw_Footprint.Services
{
	public interface INotificationTriggerService
	{
		//訂單成立通知
		Task NotifyOrderCreatedAsync(int customerId, int orderId);
		//付款成功通知
		Task NotifyPaymentSuccessAsync(int customerId, int orderId);
		//每日簽到提醒
		Task NotifyDailySignInAsync(int customerId);
		//優惠券到期提醒
		Task NotifyCouponExpiringAsync(int daysBefore = 3);
		//客服回覆通知
		Task NotifyCustomerServiceReplyAsync(int customerId, int ticketId);
		//客服工單完成通知
		Task NotifyTicketCompletedAsync(int ticketId);
        // 優惠券發放通知
        Task NotifyCouponIssuedAsync(int customerId, int couponId);
		// 訂單取消通知
		Task NotifyOrderCanceledAsync(int customerId, int orderId);
		//共用內部函式
		Task SendCustomAsync(int customerId, string title, string message, string type);
	}
}