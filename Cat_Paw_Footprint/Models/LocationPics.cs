using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class LocationPics
{
    public int? LocationID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Locations? Location { get; set; }
}
