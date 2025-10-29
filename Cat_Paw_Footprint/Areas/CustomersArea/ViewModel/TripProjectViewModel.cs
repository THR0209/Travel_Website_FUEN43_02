using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	public class TripProjectViewModel
	{
		[Display(Name = "會員ID")]
		//public virtual Customers? CustomerID { get; set; }
		public int CustomerID { get; set; }

		[Display(Name = "行程ID")]
		public int ProjectID { get; set; }

		[Display(Name = "專案名稱")]
		public string? ProjectName { get; set; }

		[Display(Name = "開始時間")]
		public DateTime? StartDate { get; set; }

		[Display(Name = "結束時間")]
		public DateTime? EndTime { get; set; }

		[Display(Name = "建立時間")]
		public DateTime? CreateTime { get; set; }

		[Display(Name = "更新時間")]
		public DateTime? UpdateTime { get; set; }

		[Display(Name = "行程日期")]
		public DateTime? TripDate { get; set; }

		[Display(Name = "行程排序")]
		public int? TripSequence { get; set; }

		[Display(Name = "開始時間")]
		public DateTime? StartTime { get; set; }

		[Display(Name = "停留分鐘數")]
		public int? StayMinute { get; set; }		

		[Display(Name = "行程類型")]    // 交通、住宿、景點、美食
		public string? TripType { get; set; }

		[Display(Name = "交通ID")]
		public int? TransportID { get; set; }

		[Display(Name = "住宿ID")]
		public int? HotelID { get; set; }

		[Display(Name = "景點ID")]
		public int? LocationID { get; set; }

		[Display(Name = "美食ID")]
		public int? RestaurantID { get; set; }

		[Display(Name = "備註")]
		public string? Notes { get; set; }
		
		public virtual Hotels? Hotel { get; set; }		
		public virtual Locations? Location { get; set; }		
		public virtual CustomerTripProjects? Project { get; set; }		
		public virtual Restaurants? Restaurant { get; set; }				
		public virtual Transportations? Transport { get; set; }
		public virtual Customers? Customer { get; set; }

		[Display(Name = "行程明細列表")]	// 讓 data.TripDetails 有型別可對應
		public List<TripDetailItem> TripDetails { get; set; } = new List<TripDetailItem>();
	}

	// 對應前端 tripDetails 陣列中的每一筆資料
	
	public class TripDetailItem{
		[Display(Name = "排序序號")]
		public int TripSequence { get; set; }

		[Display(Name = "行程類型")]
		public string TripType { get; set; } = "";

		[Display(Name = "對應目標ID")]
		public int TripTargetID { get; set; }

		[Display(Name = "名稱（前端顯示用）")]
		public string? TripName { get; set; }
	}
}
