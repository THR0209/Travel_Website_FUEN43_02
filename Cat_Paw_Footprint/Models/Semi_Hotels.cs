using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Semi_Hotels
{
	[Key]
	public int SemiHotelID { get; set; }  // 預計更改的
	public int? ProductID { get; set; }

    public int? HotelID { get; set; }

    public virtual Hotels? Hotel { get; set; }

    public virtual SemiSelfProducts? Product { get; set; }
}
