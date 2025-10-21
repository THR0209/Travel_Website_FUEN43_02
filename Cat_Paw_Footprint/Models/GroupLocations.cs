using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models
{
	public class GroupLocations// 團體集合地點 與遊客發出自己定位
	{
		[Key]
		public int LocationId { get; set; }// 主鍵位置ID
		public int GroupId { get; set; }// 團體ID
		public string SenderType { get; set; } = null!;// 發送者類型 (Guide / Customer / Guest)
		public int? GuideId { get; set; }// 導遊ID
		public string? CustomerId { get; set; }// 會員ID
		public Guid? GuestId { get; set; }// 遊客ID
		public decimal Latitude { get; set; }// GPS緯度
		public decimal Longitude { get; set; }// GPS經度
		public DateTime RecordTime { get; set; }// 紀錄時間
		public string? Note { get; set; }// 備註

		public TourGroups Group { get; set; } = null!;
	}
}
