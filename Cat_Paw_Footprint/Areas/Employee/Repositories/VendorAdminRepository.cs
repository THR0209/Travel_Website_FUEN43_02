using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;     // EmployeeDbContext
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.Employee.Repositories
{
	public class VendorAdminRepository : IVendorAdminRepository
	{
		private readonly EmployeeDbContext _db;
		public VendorAdminRepository(EmployeeDbContext db)
		{
			_db = db;
		}
		public async Task<IEnumerable<VendorAdminViewModel>> GetAllVendorsAsync()//取得所有廠商資料
		{
			return await _db.Vendors
				.Select(v => new VendorAdminViewModel //我只有Vendors表
				{
					VendorId = v.VendorId,//廠商編號
					Account = v.Account,//廠商帳號
					CreateDate = v.CreateDate,//建立日期
					Status = v.Status,//帳號啟用狀態
					CompanyName = v.CompanyName,//公司名稱//等同於Vendors表的FullName
					TaxId = v.TaxId,//統一編號
					ContactName = v.ContactName,//聯絡人姓名
					Phone = v.Phone,//電話
					Email = v.Email,//電子郵件
					Address = v.Address//地址
				}).ToListAsync();
		}
		public async Task<VendorAdminViewModel?> GetVendorByAccountAsync(string account)//根據廠商帳號取得廠商資料
		{
			var vendor = await _db.Vendors
				.Where(v => v.Account == account)
				.Select(v => new VendorAdminViewModel
				{
					VendorId = v.VendorId,//廠商編號
					Account = v.Account,//廠商帳號
					CreateDate = v.CreateDate,//建立日期
					Status = v.Status,//帳號啟用狀態
					CompanyName = v.CompanyName,//公司名稱//等同於Vendors表的FullName
					TaxId = v.TaxId,//統一編號
					ContactName = v.ContactName,//聯絡人姓名
					Phone = v.Phone,//電話
					Email = v.Email,//電子郵件
					Address = v.Address//地址
				}).FirstOrDefaultAsync();
			return vendor;
		}
		public async Task<bool> UpdateVendorStatusAsync(string account, bool status)
		{
			try { 
				var vendor = await _db.Vendors.FirstOrDefaultAsync(v => v.Account == account);
					vendor.Status = status;
					await _db.SaveChangesAsync();
					return true;
			}
			catch 
			{
				return false;
			}

		}
		//GetVendorsLoginHistoryAsync
		public async Task<IEnumerable<VendorAdminViewModel>> GetVendorsLoginHistoryAsync(int VendorId)//根據廠商帳號取得該廠商的登入紀錄
		{
			return await _db.VendorLoginHistory
				.Where(vl => vl.Vendor.VendorId == VendorId)
				.Select(vl => new VendorAdminViewModel
				{
					CompanyName = vl.Vendor.CompanyName,//公司名稱//等同於Vendors表的FullName
					LoginLogID = vl.LoginLogID,//登入紀錄編號
					LoginIP = vl.LoginIP,//登入IP
					LoginTime = vl.LoginTime,//登入時間
					IsSuccessful = vl.IsSuccessful,//是否登入成功
				}).ToListAsync();
		}
	}
}
