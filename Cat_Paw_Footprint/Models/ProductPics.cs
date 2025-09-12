using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class ProductPics
{
	[Key]
	public int ProductPicID { get; set; }  // 預計更改的
	public int? ProductID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Products? Product { get; set; }
}
