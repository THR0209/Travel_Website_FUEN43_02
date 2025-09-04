using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerBlacklist
{
	[Key]
	public int BlacklistID { get; set; }

    public int? CustomerID { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? PermissionStatus { get; set; }

    public virtual CustomerProfile? Customer { get; set; }
}
