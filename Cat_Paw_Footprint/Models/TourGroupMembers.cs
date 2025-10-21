using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models
{
	public class TourGroupMembers//會員加入紀錄
	{
		[Key]
		public int GroupId { get; set; }// 主鍵
		public string CustomerId { get; set; } = null!;// 會員ID
		public DateTime JoinTime { get; set; }// 加入時間

		public TourGroups Group { get; set; } = null!;
	}
}
