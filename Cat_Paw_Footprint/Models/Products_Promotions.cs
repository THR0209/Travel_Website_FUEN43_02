using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Promotions
{
	[Key]
	public int ProductPromoID { get; set; }  // 預計更改的
	public int? PromoID { get; set; }

    public int ProductID { get; set; }

    public virtual Products Product { get; set; } = null!;

	public virtual Promotions Promotion { get; set; } = null!;
}
