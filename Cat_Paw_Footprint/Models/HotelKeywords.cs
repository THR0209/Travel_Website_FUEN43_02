using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class HotelKeywords
{
    [Key]
    public int HotelKeywordID { get; set; }  // 預計更改的

    public int? HotelID { get; set; }

    public int? KeywordID { get; set; }

    public virtual Hotels? Hotel { get; set; }

    public virtual Keywords? Keyword { get; set; }
}
