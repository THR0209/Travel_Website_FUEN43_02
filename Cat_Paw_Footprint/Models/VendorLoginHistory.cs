using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Cat_Paw_Footprint.Models
{
	public partial class VendorLoginHistory
	{
		[Key]
		public int LoginLogID { get; set; }
		public int VendorID { get; set; }                      // FK -> Vendors.VendorId
		public string? LoginIP { get; set; }                  // 登入 IP (IPv4/IPv6)
		public DateTime LoginTime { get; set; } = DateTime.Now; // 登入時間
		public bool IsSuccessful { get; set; }                  // 是否登入成功
		public virtual Vendors Vendor { get; set; } = null!;
	}
}
//CREATE TABLE VendorLoginHistory (
//    LoginLogID INT IDENTITY(1,1) PRIMARY KEY,
//	VendorID INT NOT NULL,                      -- FK -> Vendors.VendorId
//    LoginIP NVARCHAR(45) NULL,                  -- 登入 IP (IPv4/IPv6)
//    LoginTime DATETIME2(7) NOT NULL DEFAULT GETDATE(), -- 登入時間
//    IsSuccessful BIT NOT NULL,                  -- 是否登入成功
//    CONSTRAINT FK_VendorLoginHistory_Vendors FOREIGN KEY (VendorID) REFERENCES Vendors(VendorId)
//);


