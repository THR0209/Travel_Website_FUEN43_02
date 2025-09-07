using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class EmployeeRoles
{
	[Key]
	public int RoleID { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<Employees> Employees { get; set; } = new List<Employees>();
}
