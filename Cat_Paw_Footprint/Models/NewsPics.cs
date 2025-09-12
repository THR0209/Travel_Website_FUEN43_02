using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class NewsPics
{
	[Key]
	public int NewsPicID { get; set; }  // 預計更改的

	public int? NewsID { get; set; }

    public byte[]? NewsPic { get; set; }

    public virtual NewsTable? News { get; set; }
}
