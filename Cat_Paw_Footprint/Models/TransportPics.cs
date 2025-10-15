using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class TransportPics
{
	[Key]
	public int TransportPicID { get; set; }  // 預計更改的
	public int? TransportID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Transportations? Transport { get; set; }
}
