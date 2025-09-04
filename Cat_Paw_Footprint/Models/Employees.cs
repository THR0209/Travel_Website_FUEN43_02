using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Employees
{
	[Key]
	public int EmployeeID { get; set; }

    public string? Account { get; set; }

    public string? Password { get; set; }

    public int? RoleID { get; set; }

    public DateTime? CreateDate { get; set; }

    public bool? Status { get; set; }

    public string? EmployeeCode { get; set; }

    public virtual ICollection<CustomerSupportTickets> CustomerSupportTickets { get; set; } = new List<CustomerSupportTickets>();

    public virtual EmployeeProfile? EmployeeProfile { get; set; }

    public virtual EmployeeRoles? Role { get; set; }
}
