using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class RestaurantPics
{
	[Key]
	public int RestaurantPicID { get; set; }  // 預計更改的
	public int? RestaurantID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Restaurants? Restaurant { get; set; }
}
