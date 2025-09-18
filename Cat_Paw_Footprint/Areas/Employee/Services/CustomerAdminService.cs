using Cat_Paw_Footprint.Areas.Employee.Repositories;
using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.Employee.Services
{
	public class CustomerAdminService: ICustomerAdminService
	{
		private readonly ICustomerAdminRepository _repo;
		private readonly UserManager<IdentityUser> _userManager;      // 用 Identity 管帳號
		private readonly RoleManager<IdentityRole> _roleManager;      // 如果要加角色
		private readonly EmployeeDbContext _context; // 用來存取資料庫
		public CustomerAdminService(ICustomerAdminRepository repo,
								UserManager<IdentityUser> userManager,
								RoleManager<IdentityRole> roleManager, 
								EmployeeDbContext context)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_repo = repo;
			_context = context;
		}

		public async Task<IEnumerable<CustomerAdminViewModel>> GetAllCustomersAsync()
		{
			return await _repo.GetAllCustomersAsync();
		}

		public async Task<IEnumerable<CustomerAdminViewModel>> GetAllLevelsAsync()
		{
			return await _repo.GetAllLevelsAsync();
		}

		public async Task<CustomerAdminViewModel?> GetCustomerByIdAsync(int? customerId)
		{
			return await _repo.GetCustomerByIdAsync(customerId);
		}

		public async Task<IEnumerable<CustomerAdminViewModel?>> GetLoginHistoryAsync(int? customerId)
		{
			var cus= await _repo.GetLoginHistoryAsync(customerId);
			if (cus == null || !cus.Any())
			{
				// 如果沒有找到登入紀錄，回傳空的列表而不是 null
				return Enumerable.Empty<CustomerAdminViewModel>();
			}
			return cus;
		}


		public async Task<bool> UpdateBlacklistStatusAsync(int customerId, bool isBlacklisted)// 更新黑名單狀態
		{
			var success = await _repo.UpdateBlacklistStatusAsync(customerId, isBlacklisted);
			return success;
		}

		public async Task<bool> UpdateLevelAsync(int customerId, int levelId)// 更新會員等級
		{
			
			return await _repo.UpdateLevelAsync(customerId, levelId); ;
		}


		public async Task<bool> UpdateStatusAsync(int customerId, bool status)// 更新帳號啟用狀態
		{
			
			return await _repo.UpdateStatusAsync(customerId, status); ;
		}
		public async Task<int?> GetCustomerIdByEmailAsync(string email)// 根據Email查詢CustomerId
		{
			return await _repo.GetCustomerIdByEmailAsync(email);
		}
		public async Task BatchSaveCustomersAsync(List<CustomerUpdateDto> updates)//批次更新
		{
			using var transaction = await _context.Database.BeginTransactionAsync();//開始交易
			try
			{
				var ids = updates.Select(x => x.CustomerId).ToList();
			var customers = await _repo.GetCustomersByIdsAsync(ids);

			foreach (var c in customers)
			{
				var u = updates.First(x => x.CustomerId == c.CustomerID);
				c.Level = u.LevelId;
				c.Status = u.Status;
				c.IsBlacklisted = u.IsBlacklisted;
			}

			await _context.SaveChangesAsync();
				await transaction.CommitAsync();//成功後交易結束
			}
			catch
			{
				await transaction.RollbackAsync();//失敗後交易回復
				throw;
			}
		}

	}
}
