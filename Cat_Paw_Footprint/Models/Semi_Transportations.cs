using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Semi_Transportations
{
	[Key]
	public int SemiTransID { get; set; }  // 預計更改的
	public int? ProductID { get; set; }

    public int? TransportID { get; set; }

    public virtual SemiSelfProducts? Product { get; set; }

    public virtual Transportations? Transport { get; set; }
}
