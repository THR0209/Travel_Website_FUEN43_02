using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class HotelKeywords
{
    public int? HotelID { get; set; }

    public int? KeywordID { get; set; }

    public virtual Hotels? Hotel { get; set; }

    public virtual Keywords? Keyword { get; set; }
}
