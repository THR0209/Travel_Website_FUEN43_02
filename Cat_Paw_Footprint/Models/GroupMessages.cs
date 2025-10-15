using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models
{
	public class GroupMessages
	{
		[Key]
		public int MessageId { get; set; }
		public int GroupId { get; set; }
		public string SenderType { get; set; } = null!;
		public string? CustomerId { get; set; }
		public Guid? GuestId { get; set; }
		public int? GuideId { get; set; }
		public string? Content { get; set; }
		public DateTime SendTime { get; set; }

		public TourGroups Group { get; set; } = null!;
	}
}
