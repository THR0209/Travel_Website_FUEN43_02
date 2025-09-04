using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class ProductPics
{
    public int? ProductID { get; set; }

    public byte[]? Picture { get; set; }

    public virtual Products? Product { get; set; }
}
