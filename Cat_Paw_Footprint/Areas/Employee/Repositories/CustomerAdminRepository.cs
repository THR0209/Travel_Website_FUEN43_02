using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;     // EmployeeDbContext
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.Employee.Repositories
{
	public class CustomerAdminRepository : ICustomerAdminRepository
	{
		private readonly EmployeeDbContext _db;
		public CustomerAdminRepository(EmployeeDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<CustomerAdminViewModel>> GetAllCustomersAsync()//取得所有客戶資料
		{
			return await _db.Customers
				.Select (c => new CustomerAdminViewModel 
				{ 
					CustomerID = c.CustomerID,//客戶編號
					Account = c.Account,//客戶帳號
					CreateDate = c.CreateDate,//建立日期
					IsBlacklisted = c.IsBlacklisted,//是否黑名單
					Status = c.Status,//帳號啟用狀態
					CustomerName = c.CustomerProfile.CustomerName,//客戶姓名//等同於Customers表的FullName
					IDNumber = c.CustomerProfile.IDNumber,//身分證字號
					Phone = c.CustomerProfile.Phone,//電話
					Email = c.CustomerProfile.Email,//電子郵件
					Address = c.CustomerProfile.Address,//地址
					Level = c.Level,//會員等級
					LevelName = c.LevelNavigation.LevelName//會員等級名稱
				}).ToListAsync();
			
		}

		public async Task<IEnumerable<CustomerAdminViewModel>> GetAllLevelsAsync()//取得所有會員等級資料
		{
			return await _db.Customers
				.Select (cl => new CustomerAdminViewModel 
				{ 
					Level = cl.Level,//會員等級
					LevelName = cl.LevelNavigation.LevelName//會員等級名稱
				}).ToListAsync();
		}

		public async Task<CustomerAdminViewModel?> GetCustomerByIdAsync(int? customerId)//根據客戶編號取得客戶資料
		{
			var Cus = await _db.Customers
				.Where(c => c.CustomerID == customerId)
				.Select (c => new CustomerAdminViewModel 
				{ 
					CustomerID = c.CustomerID,//客戶編號
					Account = c.Account,//客戶帳號
					CreateDate = c.CreateDate,//建立日期
					IsBlacklisted = c.IsBlacklisted,//是否黑名單
					Status = c.Status,//帳號啟用狀態
					CustomerName = c.CustomerProfile.CustomerName,//客戶姓名//等同於Customers表的FullName
					IDNumber = c.CustomerProfile.IDNumber,//身分證字號
					Phone = c.CustomerProfile.Phone,//電話
					Email = c.CustomerProfile.Email,//電子郵件
					Address = c.CustomerProfile.Address,//地址
					Level = c.Level,//會員等級
					LevelName = c.LevelNavigation.LevelName//會員等級名稱
				}).FirstOrDefaultAsync();
			return Cus;
		}

		public async Task<IEnumerable<CustomerAdminViewModel?>> GetLoginHistoryAsync(int? customerId)//根據客戶編號取得該客戶的登入紀錄
		{
			return await _db.CustomerLoginHistory
				.Where(cl => cl.CustomerID == customerId)
				.Select (cl => new CustomerAdminViewModel 
				{
					CustomerName= cl.Customer.CustomerProfile.CustomerName,//客戶姓名//等同於Customers表的FullName
					LoginLogID = cl.LoginLogID,//登入紀錄編號
					LoginIP = cl.LoginIP,//登入IP
					LoginTime = cl.LoginTime,//登入時間
					IsSuccessful = cl.IsSuccessful,//是否登入成功
				}).ToListAsync();
		}

		public async Task<bool> UpdateBlacklistStatusAsync(int customerId, bool isBlacklisted)
		{
			try {
				var customer = await _db.Customers.FindAsync(customerId);
				customer.IsBlacklisted = isBlacklisted;
				await _db.SaveChangesAsync();
				return true;
			}
			catch 
			{
				return false;
			}
		}

		public async Task<bool> UpdateLevelAsync(int customerId, int levelId)
		{
			try
			{
				var customer = await _db.Customers.FindAsync(customerId);
				customer.Level = levelId;
				await _db.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> UpdateStatusAsync(int customerId, bool status)
		{
			try
			{
				var customer = await _db.Customers.FindAsync(customerId);
				customer.Status = status;
				await _db.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
		public async Task<int?> GetCustomerIdByEmailAsync(string email)//根據信箱查詢id
		{
			var customer = await _db.CustomerProfiles
				.Where(cp => cp.Email == email)
				.Select(cp => cp.CustomerID)
				.FirstOrDefaultAsync();
			if (customer == 0) // 如果沒有找到對應的客戶，FirstOrDefaultAsync 會回傳預設值 0
			{
				return null; // 回傳 null 表示沒有找到客戶
			}
			return customer; // 回傳找到的客戶 ID
		}
	}
}
