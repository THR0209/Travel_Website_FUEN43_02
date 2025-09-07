using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Hotels
{
	[Key]
	public int HotelID { get; set; }

    public string? HotelName { get; set; }

    public string? HotelAddr { get; set; }

    public decimal? HotelLat { get; set; }

    public decimal? HotelLng { get; set; }

    public string? HotelDesc { get; set; }

    public int? RegionID { get; set; }

    public int? DistrictID { get; set; }

    public decimal? Rating { get; set; }

    public int? Views { get; set; }

    public string? HotelCode { get; set; }

    public virtual Districts? District { get; set; }

    public virtual Regions? Region { get; set; }
}
