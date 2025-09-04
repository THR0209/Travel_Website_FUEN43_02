using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerProfile
{
	[Key]
	public int CustomerProfilesID { get; set; }

    public int? CustomerID { get; set; }

    public string? CustomerName { get; set; }

    public string? IDNumber { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? CustomerProfileCode { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual ICollection<CustomerBlacklist> CustomerBlacklist { get; set; } = new List<CustomerBlacklist>();

    public virtual ICollection<CustomerOrders> CustomerOrders { get; set; } = new List<CustomerOrders>();

    public virtual ICollection<CustomerSupportTickets> CustomerSupportTickets { get; set; } = new List<CustomerSupportTickets>();
}
