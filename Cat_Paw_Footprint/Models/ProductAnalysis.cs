using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cat_Paw_Footprint.Models;

public partial class ProductAnalysis
{
    [Key]
    public int ProductAnalysisID { get; set; }

	public int? ProductID { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime? RemovalDate { get; set; }

    public virtual Products? Product { get; set; }
}
