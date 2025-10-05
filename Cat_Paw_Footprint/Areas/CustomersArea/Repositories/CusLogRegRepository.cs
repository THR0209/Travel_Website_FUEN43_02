using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Repositories
{
	public class CusLogRegRepository : ICusLogRegRepository
	{
		private readonly EmployeeDbContext _db;
		private readonly ApplicationDbContext _APdb;
		public CusLogRegRepository(EmployeeDbContext db, ApplicationDbContext aPdb)
		{
			_db = db;
			_APdb = aPdb;
		}
		public async Task<int?> CreateLoginLogAsync(int customerId, bool isSuccessful, string ip)// 建立客戶登入紀錄
		{
			var log = new CustomerLoginHistory
			{
				CustomerID = customerId,//客戶編號
				LoginIP = ip,//登入IP
				LoginTime = DateTime.Now,//登入時間
				IsSuccessful = isSuccessful//是否登入成功
			};
			_db.CustomerLoginHistory.Add(log);
			await _db.SaveChangesAsync();
			// ⚡ EF Core 會自動把新產生的 LoginLogID 回填到 log.LoginLogID
			return log.LoginLogID;
		}

		public async Task<CusLogRegDto?> GetCustomerByAccountAsync(string Account)// 根據帳號查詢客戶
		{
			
			var customer = await _APdb.Customers
		.Include(c => c.CustomerProfile)
		.Include(c => c.LevelNavigation)
		.FirstOrDefaultAsync(c => c.Account == Account);

			if (customer == null)
				return null;

			return new CusLogRegDto
			{
				CustomerId = customer.CustomerID,
				UserId = customer.UserId,
				Account = customer.Account,
				CustomerName = customer.FullName,
				Email = customer.CustomerProfile?.Email,        // 安全取值
				Phone = customer.CustomerProfile?.Phone,
				Address = customer.CustomerProfile?.Address,
				IDNumber = customer.CustomerProfile?.IDNumber,
				Level = customer.Level,
				IsBlacklisted = customer.IsBlacklisted,
				LevelName = customer.LevelNavigation?.LevelName, // 安全取值
				CreateDate = customer.CreateDate,
				FullName = customer.FullName
			};
		}

		public async Task<IEnumerable<CusLogRegDto>> GetLastLoginHistoryAsync(int? customerId)// 查詢客戶最後3次登入紀錄
		{
			return await _db.CustomerLoginHistory
				.Where(log => log.CustomerID == customerId)
				.OrderByDescending(log => log.LoginTime)
				.Take(5)
				.Select(log => new CusLogRegDto
				{
					LoginLogID = log.LoginLogID,
					LoginTime = log.LoginTime,
					LoginIP = log.LoginIP
				})
				.ToListAsync();
		}
	}
}
