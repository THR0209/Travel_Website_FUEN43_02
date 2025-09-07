using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class Semi_Locations
{
    public int? ProductID { get; set; }

    public int? LocationID { get; set; }

    public virtual Locations? Location { get; set; }

    public virtual SemiSelfProducts? Product { get; set; }
}
