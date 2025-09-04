using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class RestaurantKeywords
{
    public int? RestaurantID { get; set; }

    public int? KeywordID { get; set; }

    public virtual Keywords? Keyword { get; set; }

    public virtual Restaurants? Restaurant { get; set; }
}
