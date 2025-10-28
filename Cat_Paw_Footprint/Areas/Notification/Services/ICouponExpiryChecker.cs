namespace Cat_Paw_Footprint.Services
{
	internal interface ICouponExpiryChecker
	{
		/// <summary>
		/// 檢查即將到期的優惠券，並通知會員。
		/// 可由背景服務或手動呼叫執行。
		/// </summary>
		public interface ICouponExpiryChecker
		{
			/// <summary>
			/// 執行檢查，即將到期的優惠券。
			/// </summary>
			/// <param name="daysBefore">提前幾天通知</param>
			Task CheckExpiringCouponsAsync(int daysBefore = 3);
		}
	}
}