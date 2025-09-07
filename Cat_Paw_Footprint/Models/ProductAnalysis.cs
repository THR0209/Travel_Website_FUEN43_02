using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class ProductAnalysis
{
    public int? ProductID { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime? RemovalDate { get; set; }

    public virtual Products? Product { get; set; }
}
