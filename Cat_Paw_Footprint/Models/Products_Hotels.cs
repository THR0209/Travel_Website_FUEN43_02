using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Hotels
{
	[Key]
	public int ProductHotelID { get; set; }  // 預計更改的

	public int? ProductID { get; set; }

	public int? HotelID { get; set; }

	public int? OrderIndex { get; set; } // 914新增

	public virtual Hotels? Hotel { get; set; }

	public virtual Products? Product { get; set; }
}
