namespace Cat_Paw_Footprint.Areas.Admin.ViewModel
{
	public class DashboardViewModel
	{
		public int MonthlyOrders { get; set; }
		public int OpenTrips { get; set; }
		public int CustomerCount { get; set; }
		public int ResolvedTickets { get; set; }

		// 額外可以放成長率 / 比較資訊
		public double OrderGrowthRate { get; set; }
		public double CustomerGrowthRate { get; set; }
		public double TicketGrowthRate { get; set; }
	}
}
