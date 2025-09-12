using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Semi_Locations
{
	[Key]
	public int SemiLocationID { get; set; }  // 預計更改的
	public int? ProductID { get; set; }

    public int? LocationID { get; set; }

    public virtual Locations? Location { get; set; }

    public virtual SemiSelfProducts? Product { get; set; }
}
