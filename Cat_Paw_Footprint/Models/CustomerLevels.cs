using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerLevels
{
	[Key]
	public int Level { get; set; }

    public string? LevelName { get; set; }

    public virtual ICollection<Customers> Customers { get; set; } = new List<Customers>();
}
