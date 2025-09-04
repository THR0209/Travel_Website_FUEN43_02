using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Transportations
{
	[Key]
	public int TransportID { get; set; }

    public string? TransportName { get; set; }

    public string? TransportDesc { get; set; }

    public int? TransportPrice { get; set; }

    public decimal? Rating { get; set; }

    public int? Views { get; set; }

    public string? TransportCode { get; set; }
}
