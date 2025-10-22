using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models
{
	public class TourGroups
	{
		[Key]
		public int GroupId { get; set; }// 主鍵 團體ID
		public string GroupCode { get; set; } = null!;// 團體代碼10碼
		public string GroupName { get; set; } = null!;// 團體名稱
		public string? Destination { get; set; }// 目的地
		public DateTime? StartDate { get; set; }// 開始日期
		public DateTime? EndDate { get; set; }// 結束日期
		public int GuideId { get; set; }// 導遊ID
		public DateTime CreateTime { get; set; }// 建立時間
		public string Status { get; set; } = "Active";// 狀態 (Active, Inactive, Completed)

		// 導覽屬性
		public ICollection<TourGroupGuests>? Guests { get; set; }
		public ICollection<TourGroupMembers>? Members { get; set; }
		public ICollection<GroupMessages>? Messages { get; set; }
		public ICollection<GroupPhotos>? Photos { get; set; }
		public ICollection<GroupLocations>? Locations { get; set; }
	}
}
