using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class HotelPics
{
    public int? HotelID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Hotels? Hotel { get; set; }
}
