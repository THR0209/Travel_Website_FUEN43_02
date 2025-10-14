namespace Cat_Paw_Footprint.Models
{
	public class TourGroups
	{
		public int GroupId { get; set; }
		public string GroupCode { get; set; } = null!;
		public string GroupName { get; set; } = null!;
		public string? Destination { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public int GuideId { get; set; }
		public DateTime CreateTime { get; set; }
		public string Status { get; set; } = "Active";

		// 導覽屬性
		public ICollection<TourGroupGuests>? Guests { get; set; }
		public ICollection<TourGroupMembers>? Members { get; set; }
		public ICollection<GroupMessages>? Messages { get; set; }
		public ICollection<GroupPhotos>? Photos { get; set; }
		public ICollection<GroupLocations>? Locations { get; set; }
	}
}
