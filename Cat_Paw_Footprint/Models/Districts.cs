using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Districts
{
	[Key]
	public int DistrictID { get; set; }

    public string? DistrictName { get; set; }

    public virtual ICollection<Hotels> Hotels { get; set; } = new List<Hotels>();

    public virtual ICollection<Locations> Locations { get; set; } = new List<Locations>();

    public virtual ICollection<Restaurants> Restaurants { get; set; } = new List<Restaurants>();
}
