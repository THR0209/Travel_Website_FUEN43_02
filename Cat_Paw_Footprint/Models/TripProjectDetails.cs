using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class TripProjectDetails
{
    public int? ProjectID { get; set; }

    public DateTime? TripDate { get; set; }

    public int? TripSequence { get; set; }

    public DateTime? StartTime { get; set; }

    public int? StayMinute { get; set; }

    public int? TransportID { get; set; }

    public string? TripType { get; set; }

    public int? HotelID { get; set; }

    public int? LocationID { get; set; }

    public int? RestaurantID { get; set; }

    public string? Notes { get; set; }

    public virtual Hotels? Hotel { get; set; }

    public virtual Locations? Location { get; set; }

    public virtual CustomerTripProjects? Project { get; set; }

    public virtual Restaurants? Restaurant { get; set; }

    public virtual Transportations? Transport { get; set; }
}
