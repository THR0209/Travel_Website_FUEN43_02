using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class FAQs
{
	[Key]
	public int FAQID { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }

    public int? CategoryID { get; set; }

	public bool IsActive { get; set; }

	public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public virtual FAQCategorys? Category { get; set; }
}
