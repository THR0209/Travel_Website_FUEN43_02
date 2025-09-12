using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Transportations
{
    [Key]
    public int ProductTransID { get; set; }  // 預計更改的
    public int? ProductID { get; set; }

    public int? TransportID { get; set; }

    public string? TransportInfo { get; set; }

    public DateTime? TransportTime { get; set; }

    public virtual Products? Product { get; set; }

    public virtual Transportations? Transport { get; set; }
}
