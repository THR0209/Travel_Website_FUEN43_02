using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Products
{
	[Key]
	public int ProductID { get; set; }

    public string? ProductName { get; set; }

    public int? RegionID { get; set; }

    public string? ProductDesc { get; set; }

    public int? ProductPrice { get; set; }

    public string? ProductNote { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MaxPeople { get; set; }

    public byte[]? ProductImage { get; set; }

    public string? ProductCode { get; set; }

    public virtual ICollection<CustomerOrders> CustomerOrders { get; set; } = new List<CustomerOrders>();

    public virtual Regions? Region { get; set; }
}
