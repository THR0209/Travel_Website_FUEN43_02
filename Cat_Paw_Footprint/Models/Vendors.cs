using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Cat_Paw_Footprint.Models
{
	public partial class Vendors
	{
		[Key]
		public int VendorId { get; set; }
		public string UserId { get; set; } = null!;// AspNetUsers.Id
		public string Account { get; set; } = null!;
		public string CompanyName { get; set; } = null!;
		public string? Email { get; set; }
		public string? ContactName { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
		public string? TaxId { get; set; }
		public bool Status { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime? UpdateDate { get; set; }
		public IdentityUser User { get; set; } = null!;
		public virtual ICollection<VendorLoginHistory> VendorLoginHistory { get; set; } = new List<VendorLoginHistory>();

	}
}
//CREATE TABLE Vendors (
//    VendorId INT IDENTITY(1,1) PRIMARY KEY,
//	UserId NVARCHAR(450) NOT NULL,       -- AspNetUsers.Id
//    Account NVARCHAR(50) NOT NULL,       -- AspNetUsers.UserName (van0001, van0002…)
//    CompanyName NVARCHAR(100) NOT NULL,  -- 廠商名稱
//    Email NVARCHAR(100) NULL,            -- 聯絡信箱
//    ContactName NVARCHAR(50) NULL,       -- 聯絡人
//    Phone NVARCHAR(20) NULL,             -- 聯絡電話
//    Address NVARCHAR(200) NULL,          -- 公司地址
//    TaxId NVARCHAR(20) NULL,             -- 統編
//    Status BIT NOT NULL DEFAULT 1,       -- 啟用/停用
//    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
//	UpdateDate DATETIME NULL,

//	CONSTRAINT UQ_Vendors_UserId UNIQUE (UserId),
//	CONSTRAINT UQ_Vendors_Account UNIQUE (Account),
//	CONSTRAINT FK_Vendors_AspNetUsers FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
//);
