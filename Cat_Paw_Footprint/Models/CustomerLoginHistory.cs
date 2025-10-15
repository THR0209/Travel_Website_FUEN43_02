using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class CustomerLoginHistory
{
	[Key]
	public int LoginLogID { get; set; }//登入紀錄ID

	public int? CustomerID { get; set; }//會員ID

	public string? LoginIP { get; set; }//登入IP

	public DateTime? LoginTime { get; set; }//登入時間

	public bool? IsSuccessful { get; set; }//是否成功

	public virtual Customers? Customer { get; set; }
}
