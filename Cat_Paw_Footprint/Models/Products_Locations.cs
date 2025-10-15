using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Locations
{
	[Key]
	public int ProductLocationID { get; set; }  // 預計更改的
	public int? ProductID { get; set; }

	public int? LocationID { get; set; }

	public int? DayNumber { get; set; } // 914新增

	public int? OrderIndex { get; set; } // 914新增

	public virtual Locations? Location { get; set; }

	public virtual Products? Product { get; set; }
}
