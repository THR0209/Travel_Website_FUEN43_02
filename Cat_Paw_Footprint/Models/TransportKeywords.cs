using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class TransportKeywords
{
    public int? TransportID { get; set; }

    public int? KeywordID { get; set; }

    public virtual Keywords? Keyword { get; set; }

    public virtual Transportations? Transport { get; set; }
}
