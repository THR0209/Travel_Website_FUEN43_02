using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class HotelPics
{

	[Key]
	public int HotelPicID { get; set; }  // 預計更改的
	public int? HotelID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Hotels? Hotel { get; set; }

}
