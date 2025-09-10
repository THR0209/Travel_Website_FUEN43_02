using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Customers
{
	[Key]
	public int CustomerID { get; set; }

    public string? Account { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public int? Level { get; set; }

    public DateTime? CreateDate { get; set; }

    public bool? Status { get; set; }

    public bool? IsBlacklisted { get; set; }

    public string? CustomerCode { get; set; }
	public string? UserId { get; set; }// ★ 新增：指向 AspNetUsers(Id)

	public virtual ICollection<CustomerLoginHistory> CustomerLoginHistory { get; set; } = new List<CustomerLoginHistory>();

    public virtual CustomerProfile? CustomerProfile { get; set; }

    public virtual CustomerLevels? LevelNavigation { get; set; }
	public IdentityUser User { get; set; }
}
