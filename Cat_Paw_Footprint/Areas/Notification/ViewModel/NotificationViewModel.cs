using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	/// <summary>
	/// 通知顯示用 ViewModel
	/// </summary>
	public class NotificationViewModel
	{
		[Key]
		public int NotificationID { get; set; }

		public int CustomerID { get; set; }

		public string Title
		{ get; set; } = null!;
		public string Message { get; set; } = null!;

		public string Type { get; set; } = null!;

		public bool IsRead { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime? ReadAt { get; set; }

		public virtual Customers Customer { get; set; } = null!;
	}
}
