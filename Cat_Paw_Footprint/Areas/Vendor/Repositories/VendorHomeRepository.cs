using Cat_Paw_Footprint.Areas.Vendor.ViewModel;
using Cat_Paw_Footprint.Data;     // EmployeeDbContext
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;


namespace Cat_Paw_Footprint.Areas.Vendor.Repositories
{
	public class VendorHomeRepository: IVendorHomeRepository
	{
		private readonly EmployeeDbContext _db;
		public VendorHomeRepository(EmployeeDbContext db)
		{
			_db = db;
		}
		public async Task<VendorHomeViewModel?> GetVendorByAccountAsync(string account)//根據帳號查詢廠商
		{
			return await _db.Vendors
				.Where(v => v.Account == account)
				.Select(v => new VendorHomeViewModel
				{
					VendorId = v.VendorId,//廠商編號
					Account = v.Account,//廠商帳號
					CompanyName = v.CompanyName,//廠商名稱
					Email = v.Email,//聯絡信箱
					ContactName = v.ContactName,//聯絡人
					Phone = v.Phone,//聯絡電話
					Address = v.Address,//公司地址
					TaxId = v.TaxId,//統編
					Status = v.Status,//啟用/停用
					CreateDate = v.CreateDate,//建立日期
					UpdateDate = v.UpdateDate//廠商資料最後更新日期
				})
				.FirstOrDefaultAsync();
		}


		public async Task<int?> CreateLoginLogAsync(int vendorId, bool isSuccessful, string ip)//建立廠商登入紀錄
		{
			var log = new VendorLoginHistory
			{
				VendorID = vendorId,//廠商編號
				LoginIP = ip,//登入IP
				LoginTime = DateTime.Now,//登入時間
				IsSuccessful = isSuccessful//是否登入成功
			};

			_db.VendorLoginHistory.Add(log);
			await _db.SaveChangesAsync();

			// ⚡ EF Core 會自動把新產生的 LoginLogID 回填到 log.LoginLogID
			return log.LoginLogID;
		}
		public async Task<IEnumerable<VendorHomeViewModel?>> GetLast3LoginLogsAsync(int vendorId)//查詢廠商前3次登入狀況
			{
			return await _db.VendorLoginHistory
				.Where(log => log.VendorID == vendorId)
				.OrderByDescending(log => log.LoginTime)
				.Take(3)
				.Select(log => new VendorHomeViewModel
				{
					LoginLogID = log.LoginLogID,//登入紀錄編號
					LoginTime = log.LoginTime,//登入時間
					IsSuccessful = log.IsSuccessful,//是否登入成功
					LoginIP = log.LoginIP//登入IP
				})
				.ToListAsync();
		}
		public async Task<bool?> LockVendorAccountAsync(int vendorId)//連續登入失敗滿3次後廠商鎖帳
			{
			var vendor = await _db.Vendors.FindAsync(vendorId);
			if (vendor == null)
			{
				return null; // 廠商不存在
			}
			vendor.Status = false; // 鎖帳
			_db.Vendors.Update(vendor);
			var result = await _db.SaveChangesAsync();
			return result > 0; // 返回是否成功更新
		}
		public async Task<bool?> UpdateLastLoginAsync(int vendorId, DateTime lastLogin, string ip)//更新最後登入時間與IP
			{
			var vendor = await _db.Vendors.FindAsync(vendorId);
			if (vendor == null)
			{
				return null; // 廠商不存在
			}
			vendor.UpdateDate = lastLogin; // 更新最後登入時間
			// 如果有其他欄位需要更新，可以在這裡添加
			_db.Vendors.Update(vendor);
			var result = await _db.SaveChangesAsync();
			return result > 0; // 返回是否成功更新
		}
	}
}
