using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class SemiSelfProducts
{
	[Key]
	public int ProductID { get; set; }

    public string? ProjectName { get; set; }

    public int? Region { get; set; }

    public string? ProductDesc { get; set; }

    public int? ProductPrice { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

	[DisplayName("狀態")]
	public bool? IsActive { get; set; }

	[DisplayName("瀏覽次數")]
	public int? Views { get; set; }

	public string? Notes { get; set; }

    public int? MaxPeople { get; set; }

    public string? ProductCode { get; set; }

    public virtual ICollection<Semi_Hotels> SemiHotels { get; set; } = new List<Semi_Hotels>();

    public virtual ICollection<Semi_Locations> SemiLocations { get; set; } = new List<Semi_Locations>();

    public virtual ICollection<Semi_Transportations> SemiTransportations { get; set; } = new List<Semi_Transportations>();
}
