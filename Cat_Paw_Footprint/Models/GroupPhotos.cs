using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models
{
	public class GroupPhotos// 上傳照片到群組
	{
		[Key]
		public int PhotoId { get; set; }// 主鍵照片ID
		public int GroupId { get; set; }// 團體ID
		public string UploaderType { get; set; } = null!;// 上傳者類型 (Customer / Guest / Guide)
		public string? CustomerId { get; set; }// 會員ID
		public Guid? GuestId { get; set; }// 遊客ID
		public int? GuideId { get; set; }// 導遊ID
		public string? FilePath { get; set; }// 照片路徑
		public decimal? Latitude { get; set; }// GPS緯度
		public decimal? Longitude { get; set; }// GPS經度
		public DateTime UploadTime { get; set; }// 上傳時間
		public bool IsApproved { get; set; }// 是否審核通過

		public TourGroups Group { get; set; } = null!;
	}
}
