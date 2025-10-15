using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class RestaurantKeywords
{
	[Key]
	public int RestaurantKeywordID { get; set; }  // 預計更改的
	public int? RestaurantID { get; set; }

    public int? KeywordID { get; set; }

    public virtual Keywords? Keyword { get; set; }

    public virtual Restaurants? Restaurant { get; set; }
}
