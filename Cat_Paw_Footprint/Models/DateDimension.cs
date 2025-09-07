using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class DateDimension
{
	[Key]
	public int DateID { get; set; }

    public DateTime? Date { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public int? Day { get; set; }

    public int? Week { get; set; }

    public int? DayOfWeek { get; set; }

    public string? DayName { get; set; }

    public bool? IsWeekend { get; set; }

    public bool? IsHoliday { get; set; }
}
