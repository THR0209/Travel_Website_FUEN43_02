using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Restaurants
{
	[Key]
	public int RestaurantID { get; set; }

    public string? RestaurantName { get; set; }

    public string? RestaurantAddr { get; set; }

    public decimal? RestaurantLat { get; set; }

    public decimal? RestaurantLng { get; set; }

    public string? RestaurantDesc { get; set; }

    public int? RegionID { get; set; }

    public int? DistrictID { get; set; }

    public decimal? Rating { get; set; }

    public int? Views { get; set; }

    public virtual Districts? District { get; set; }

    public virtual Regions? Region { get; set; }
}
