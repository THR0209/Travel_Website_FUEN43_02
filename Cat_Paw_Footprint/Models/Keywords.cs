using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Keywords
{
	[Key]
	public int KeywordID { get; set; }

    public string? Keyword { get; set; }

    public int? Views { get; set; }
}
