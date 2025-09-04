using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Regions
{
	[Key]
	public int RegionID { get; set; }

    public string? RegionName { get; set; }

    public virtual ICollection<Hotels> Hotels { get; set; } = new List<Hotels>();

    public virtual ICollection<Locations> Locations { get; set; } = new List<Locations>();

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual ICollection<Restaurants> Restaurants { get; set; } = new List<Restaurants>();
}
