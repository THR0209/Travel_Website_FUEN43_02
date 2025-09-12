using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class TransportKeywords
{
	[Key]
	public int TransportKeywordID { get; set; }  // 預計更改的
	public int? TransportID { get; set; }

    public int? KeywordID { get; set; }

    public virtual Keywords? Keyword { get; set; }

    public virtual Transportations? Transport { get; set; }
}
