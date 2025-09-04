using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class Semi_Transportations
{
    public int? ProductID { get; set; }

    public int? TransportID { get; set; }

    public virtual SemiSelfProducts? Product { get; set; }

    public virtual Transportations? Transport { get; set; }
}
