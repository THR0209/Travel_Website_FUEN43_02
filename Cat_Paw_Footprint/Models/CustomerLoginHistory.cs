using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerLoginHistory
{
	[Key]
	public int LoginLogID { get; set; }

    public int? CustomerID { get; set; }

    public string? LoginIP { get; set; }

    public DateTime? LoginTime { get; set; }

    public bool? IsSuccessful { get; set; }

    public virtual Customers? Customer { get; set; }
}
