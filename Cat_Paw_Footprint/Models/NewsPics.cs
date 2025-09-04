using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class NewsPics
{
    public int? NewsID { get; set; }

    public byte[]? NewsPic { get; set; }

    public virtual NewsTable? News { get; set; }
}
