using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerTripProjects
{
    public int? CustomerID { get; set; }
	[Key]
	public int ProjectID { get; set; }

    public string? ProjectName { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }
}
