using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.Admin.ViewModel
{
	[DateRange("PublishTime", "ExpireTime", ErrorMessage = "發佈時間不可大於到期時間")]
	public class NewsEditViewModel
    {  
        public int NewsID { get; set; }

        [Display(Name = "消息標題")]
        public string? NewsTitle { get; set; }
        [Display(Name = "消息內容")]
        public string? NewsContent { get; set; }
        [Display(Name = "發佈時間")]
        public DateTime? PublishTime { get; set; }
        [Display(Name = "到期時間")]
        public DateTime? ExpireTime { get; set; }

        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; }

        [Display(Name = "員工姓名")]
        public string? EmployeeName { get; set; }

        public int? EmployeeID { get; set; }

        public virtual EmployeeProfile? Employee { get; set; }
    }
}
