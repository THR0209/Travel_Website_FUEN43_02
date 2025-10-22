using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models
{
	public class GroupMessages// 群組訊息
	{
		[Key]
		public int MessageId { get; set; }// 主鍵訊息ID
		public int GroupId { get; set; }// 團體ID
		public string SenderType { get; set; } = null!;// 發送者類型 (Customer / Guest / Guide)
		public string? CustomerId { get; set; }// 會員ID
		public Guid? GuestId { get; set; }// 遊客ID
		public int? GuideId { get; set; }// 導遊ID
		public string? Content { get; set; }// 訊息內容
		public DateTime SendTime { get; set; }// 發送時間

		public TourGroups Group { get; set; } = null!;
	}
}
