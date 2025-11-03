namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	public class TourGroupAllDto
	{
	}
	public class CusJoinDto
	{
		public string GroupCode { get; set; } = null!;
		public int CustomerId { get; set; }
		public string? CustomerName { get; set; }
	}

	public class GuestJoinDto
	{
		public string GroupCode { get; set; } = null!;
		public string DeviceId { get; set; } = null!;
		public string? TemporaryName { get; set; }
		public string? GuestId { get; set; } // 可選，若前端有保 GuestId 則給
	}
}
