using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Transportations
{
    public int? ProductID { get; set; }

    public int? TransportID { get; set; }

    public string? TransportInfo { get; set; }

    public DateTime? TransportTime { get; set; }

    public virtual Products? Product { get; set; }

    public virtual Transportations? Transport { get; set; }
}
