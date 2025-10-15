using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models
{
	public class TourGroupMembers
	{
		[Key]
		public int GroupId { get; set; }
		public string CustomerId { get; set; } = null!;
		public DateTime JoinTime { get; set; }

		public TourGroups Group { get; set; } = null!;
	}
}
