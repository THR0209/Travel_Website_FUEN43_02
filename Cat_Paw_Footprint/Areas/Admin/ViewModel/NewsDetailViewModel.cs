using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.Admin.ViewModel
{
    public class NewsDetailViewModel
    {
        public int NewsID { get; set; }
        [Display(Name = "消息標題")]
        public string NewsTitle { get; set; }

        [Display(Name = "消息內容")]
        public string NewsContent { get; set; }

        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; }

        [Display(Name = "發布時間")]
        public DateTime? PublishTime { get; set; }

        [Display(Name = "到期時間")]
        public DateTime? ExpireTime { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "更新時間")]
        public DateTime? UpdateTime { get; set; }

        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
    }
}
