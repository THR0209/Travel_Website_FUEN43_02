using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Keywords
{
	[Key]
	public int ProductKeywordID { get; set; }

	public int? ProductID { get; set; }

	public int? KeywordID { get; set; }

	public virtual Keywords? Keyword { get; set; }

	public virtual Products? Product { get; set; }
}
