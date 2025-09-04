using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cat_Paw_Footprint.Models;
[Table("EmployeeProfile", Schema = "dbo")]
public partial class EmployeeProfile
{
	[Key]
	public int EmployeeProfileID { get; set; }

    public int? EmployeeID { get; set; }

    public string? EmployeeName { get; set; }

    public string? IDNumber { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public byte[]? Photo { get; set; }

    public string? EmployeeProfileCode { get; set; }

    public virtual Employees? Employee { get; set; }

    public virtual ICollection<NewsTable> NewsTable { get; set; } = new List<NewsTable>();
}
