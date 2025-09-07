using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class FAQCategorys
{
	[Key]
	public int CategoryID { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<FAQs> FAQs { get; set; } = new List<FAQs>();
}
