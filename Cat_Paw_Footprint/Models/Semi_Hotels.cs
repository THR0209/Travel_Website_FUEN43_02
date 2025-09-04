using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class Semi_Hotels
{
    public int? ProductID { get; set; }

    public int? HotelID { get; set; }

    public virtual Hotels? Hotel { get; set; }

    public virtual SemiSelfProducts? Product { get; set; }
}
