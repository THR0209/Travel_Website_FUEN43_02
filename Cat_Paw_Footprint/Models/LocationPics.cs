using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class LocationPics
{
	[Key]
	public int LocationPicID { get; set; }  // 預計更改的
	public int? LocationID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Locations? Location { get; set; }
}
