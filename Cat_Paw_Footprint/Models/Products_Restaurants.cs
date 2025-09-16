using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Restaurants
{
	[Key]
	public int ProductRestaurantID { get; set; }  // 預計更改的

	public int? ProductID { get; set; }


	public int? RestaurantID { get; set; }

	public string? MealType { get; set; } // 914新增

	public int? OrderIndex { get; set; } // 914新增

	public virtual Products? Product { get; set; }

	public virtual Restaurants? Restaurant { get; set; }
}
