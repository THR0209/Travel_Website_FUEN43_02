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

	public bool? IsActive { get; set; }

	public virtual ICollection<TransportKeywords> TransportKeywords { get; set; } = new List<TransportKeywords>();

	public virtual ICollection<TransportPics> TransportPics { get; set; } = new List<TransportPics>();
}
