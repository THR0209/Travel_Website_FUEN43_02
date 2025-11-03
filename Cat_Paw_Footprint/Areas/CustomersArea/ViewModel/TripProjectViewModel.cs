using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	public class TripProjectViewModel
	{
		[Display(Name = "會員ID")]
		public int? CustomerID { get; set; }

		[Display(Name = "行程ID")]
		public int ProjectID { get; set; }

		[Display(Name = "專案名稱")]
		public string? ProjectName { get; set; }

		[Display(Name = "建立時間")]
		public DateTime? CreateTime { get; set; }

		[Display(Name = "更新時間")]
		public DateTime? UpdateTime { get; set; }

		[Display(Name ="總天數")]
		public int? TotalDays { get; set; }

		[Display(Name = "行程日期")]
		public int? TripDate { get; set; }

		[Display(Name = "行程排序")]
		public int? TripSequence { get; set; }

		[Display(Name = "開始時間")]
		public string? StartTime { get; set; } // 使用 string 以接收 "HH:mm" 格式

		[Display(Name = "停留分鐘數")]
		public int? StayMinute { get; set; }		

		[Display(Name = "行程類型")]    // 交通、住宿、景點、美食
		public string? TripType { get; set; }

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

		/* 用來接收多筆明細資料（前端的 Details 陣列） */
		[Display(Name = "行程明細清單")]
		public List<TripProjectViewModel>? Details { get; set; } // 此屬性名稱必須與前端 JSON "Details" 完全一致
	}
}
