using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Locations
{
	[Key]
	public int LocationID { get; set; }

    public string? LocationName { get; set; }

    public string? LocationAddr { get; set; }

    public decimal? LocationLat { get; set; }

    public decimal? LocationLng { get; set; }

    public string? LocationDesc { get; set; }

    public int? LocationPrice { get; set; }

    public int? RegionID { get; set; }

    public int? DistrictID { get; set; }

    public decimal? Rating { get; set; }

    public int? Views { get; set; }

    public string? LocationCode { get; set; }

	public bool? IsActive { get; set; }

	public virtual Districts? District { get; set; }

    public virtual Regions? Region { get; set; }

    public virtual ICollection<LocationPics> LocationPics { get; set; } = new List<LocationPics>();

    public virtual ICollection<LocationKeywords> LocationKeywords { get; set; } = new List<LocationKeywords>();
}
