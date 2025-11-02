namespace Cat_Paw_Footprint.Services
{
	public interface INotificationTriggerService
	{
		Task NotifyOrderCreatedAsync(int customerId, int orderId);
		Task NotifyCustomerServiceReplyAsync(int customerId, int ticketId);
		Task NotifyDailySignInAsync(int customerId);
		Task NotifyCouponExpiringAsync(int daysBefore = 3);
		Task SendCustomAsync(int customerId, string title, string message, string type);
		Task NotifyPaymentSuccessAsync(int customerId, int orderId);
	}
}