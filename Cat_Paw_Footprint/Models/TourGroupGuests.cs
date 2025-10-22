using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models
{
	public class TourGroupGuests// 訪客團體成員
	{
		[Key]
		public Guid GuestId { get; set; }// 主鍵訪客id
		public int GroupId { get; set; }// 團體id
		public string? TemporaryName { get; set; }// 臨時名稱
		public string? DeviceId { get; set; }// 設備識別
		public DateTime JoinTime { get; set; }// 加入時間
		public DateTime? LastActive { get; set; }// 最後活動時間
		public bool IsMember { get; set; }// 是否為正式成員

		public TourGroups Group { get; set; } = null!;
	}
}
