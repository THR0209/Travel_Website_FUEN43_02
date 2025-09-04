using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class RestaurantPics
{
    public int? RestaurantID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Restaurants? Restaurant { get; set; }
}
