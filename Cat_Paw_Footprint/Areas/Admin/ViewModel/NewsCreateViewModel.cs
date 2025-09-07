using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.Admin.ViewModel
{
    public class NewsCreateViewModel
    {
        public int NewsID { get; set; }

        public string? NewsTitle { get; set; }

        public string? NewsContent { get; set; }

        public DateTime? PublishTime { get; set; }

        public DateTime? ExpireTime { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int? EmployeeID { get; set; }

        public virtual EmployeeProfile? Employee { get; set; }
    }
}
