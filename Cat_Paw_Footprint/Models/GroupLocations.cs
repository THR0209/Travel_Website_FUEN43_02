namespace Cat_Paw_Footprint.Models
{
	public class GroupLocations
	{
		public int LocationId { get; set; }
		public int GroupId { get; set; }
		public string SenderType { get; set; } = null!;
		public int? GuideId { get; set; }
		public string? CustomerId { get; set; }
		public Guid? GuestId { get; set; }
		public decimal Latitude { get; set; }
		public decimal Longitude { get; set; }
		public DateTime RecordTime { get; set; }
		public string? Note { get; set; }

		public TourGroups Group { get; set; } = null!;
	}
}
