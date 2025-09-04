using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Restaurants
{
    public int? ProductID { get; set; }

    public int? RestaurantID { get; set; }

    public virtual Products? Product { get; set; }

    public virtual Restaurants? Restaurant { get; set; }
}
