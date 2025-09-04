using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Hotels
{
    public int? ProductID { get; set; }

    public int? HotelID { get; set; }

    public virtual Hotels? Hotel { get; set; }

    public virtual Products? Product { get; set; }
}
