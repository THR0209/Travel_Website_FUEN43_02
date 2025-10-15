using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Keywords
{
	[Key]
	public int KeywordID { get; set; }

	[Display(Name ="關鍵字")]
    public string? Keyword { get; set; }

	[Display(Name ="瀏覽次數")]
    public int? Views { get; set; }
}
