using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class LocationKeywords
{
    public int? LocationID { get; set; }

    public int? KeywordID { get; set; }

    public virtual Keywords? Keyword { get; set; }

    public virtual Locations? Location { get; set; }
}
