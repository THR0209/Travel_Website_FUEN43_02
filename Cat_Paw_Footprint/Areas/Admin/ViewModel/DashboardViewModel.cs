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

	public class SalesReportDto
	{
		public string X { get; set; } // X 軸 (日期/月份/季度/年份)
		public decimal Y { get; set; } // Y 軸 (銷售額)
	}


}
