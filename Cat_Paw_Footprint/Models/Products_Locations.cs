using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Locations
{
    public int? ProductID { get; set; }

    public int? LocationID { get; set; }

    public virtual Locations? Location { get; set; }

    public virtual Products? Product { get; set; }
}
