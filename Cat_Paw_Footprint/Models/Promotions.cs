using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Promotions
{
	[Key]
	public int PromoID { get; set; }

    public string? PromoName { get; set; }

    public string? PromoDesc { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }
}
