using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class LocationKeywords
{

	[Key]
	public int LocationKeywordID { get; set; }  // 預計更改的
	public int? LocationID { get; set; }

    public int? KeywordID { get; set; }

    public virtual Keywords? Keyword { get; set; }

    public virtual Locations? Location { get; set; }
}
