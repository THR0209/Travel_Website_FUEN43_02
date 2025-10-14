namespace Cat_Paw_Footprint.Models
{
	public class GroupPhotos
	{
		public int PhotoId { get; set; }
		public int GroupId { get; set; }
		public string UploaderType { get; set; } = null!;
		public string? CustomerId { get; set; }
		public Guid? GuestId { get; set; }
		public int? GuideId { get; set; }
		public string? FilePath { get; set; }
		public decimal? Latitude { get; set; }
		public decimal? Longitude { get; set; }
		public DateTime UploadTime { get; set; }
		public bool IsApproved { get; set; }

		public TourGroups Group { get; set; } = null!;
	}
}
