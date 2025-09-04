using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class TransportPics
{
    public int? TransportID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Transportations? Transport { get; set; }
}
