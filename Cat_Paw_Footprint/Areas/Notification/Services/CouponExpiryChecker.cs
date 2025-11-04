using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Areas.Notification.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Cat_Paw_Footprint.Services
{
	/// <summary>
	/// 背景排程：每日檢查即將到期的優惠券
	/// </summary>
	public class CouponExpiryChecker : BackgroundService, ICouponExpiryChecker
	{
		private readonly IServiceProvider _provider;
		private readonly ILogger<CouponExpiryChecker> _logger;

		public CouponExpiryChecker(IServiceProvider provider, ILogger<CouponExpiryChecker> logger)
		{
			_provider = provider;
			_logger = logger;
		}

		/// <summary>
		/// 背景排程會自動執行，每 24 小時跑一次
		/// </summary>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("🎯 CouponExpiryChecker 背景任務啟動");

			while (!stoppingToken.IsCancellationRequested)
			{
				await CheckExpiringCouponsAsync();
				await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
			}
		}

		/// <summary>
		/// 實際檢查優惠券並發送通知
		/// </summary>
		public async Task CheckExpiringCouponsAsync(int daysBefore = 3)
		{
			using var scope = _provider.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<webtravel2Context>();
			var notifSvc = scope.ServiceProvider.GetRequiredService<INotificationService>();

			var now = DateTime.Now;
			var soon = now.AddDays(daysBefore);

			var expiring = await db.CustomerCouponsRecords
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
                    await notifSvc.AddNotificationAsync(
                        (int)r.CustomerID,
                        "優惠券即將到期",
                        $"您的優惠券「{r.Coupon.CouponDesc}」將於 {r.Coupon.EndDate:MM/dd} 到期，請盡快使用！",
                        "優惠活動"
                    );
                }
            }


            _logger.LogInformation($"✅ 優惠券檢查完成，共發送 {expiring.Count} 筆通知。");
		}
	}
}
