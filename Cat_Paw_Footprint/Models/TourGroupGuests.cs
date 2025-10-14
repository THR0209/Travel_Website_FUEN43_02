namespace Cat_Paw_Footprint.Models
{
	public class TourGroupGuests
	{
		public Guid GuestId { get; set; }
		public int GroupId { get; set; }
		public string? TemporaryName { get; set; }
		public string? DeviceId { get; set; }
		public DateTime JoinTime { get; set; }
		public DateTime? LastActive { get; set; }
		public bool IsMember { get; set; }

		public TourGroups Group { get; set; } = null!;
	}
}
