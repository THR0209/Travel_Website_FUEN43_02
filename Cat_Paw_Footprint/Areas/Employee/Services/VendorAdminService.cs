using Cat_Paw_Footprint.Areas.Employee.Repositories;
using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Cat_Paw_Footprint.Areas.Employee.Services
{
	public class VendorAdminService : IVendorAdminService
	{
		private readonly IVendorAdminRepository _repo;
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;      // 用 Identity 管帳號
		private readonly RoleManager<IdentityRole> _roleManager;      // 如果要加角色

		public VendorAdminService(ApplicationDbContext context, IVendorAdminRepository repo, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_repo = repo;
			_userManager = userManager;
			_roleManager = roleManager;
		}
		public async Task<IEnumerable<VendorAdminViewModel>> GetAllVendorsAsync()// 查詢所有廠商
		{
			return await _repo.GetAllVendorsAsync();
		}
		public async Task<VendorAdminViewModel?> GetVendorByAccountAsync(string account)// 根據帳號查詢廠商詳細資料
		{
			return await _repo.GetVendorByAccountAsync(account);
		}
		public async Task<bool> UpdateStatusAsync(string account, bool status)// 更新廠商啟用狀態
		{
			var success = await _repo.UpdateVendorStatusAsync(account, status);
			return success;
		}
		public async Task<IEnumerable<VendorAdminViewModel>> GetVendorsLoginHistoryAsync(int VendorId)// 查詢廠商登入歷史記錄
		{
			return await _repo.GetVendorsLoginHistoryAsync(VendorId);
		}
		public async Task<bool> RegisterVendorAsync(VendorAdminViewModel model)//廠商註冊
		{
			using var transaction =await _context.Database.BeginTransactionAsync();//開始交易
			try {


				var defaultPassword = "Travel@123";
				// 計算流水號帳號
				var count = await _context.Vendors.CountAsync();
				var newAccount = $"ven{(count + 1).ToString("D4")}";

				var user = new IdentityUser
				{
					UserName = newAccount,
					Email = model.Email
				};
				var result = await _userManager.CreateAsync(user, defaultPassword);
				if (!result.Succeeded)
				{
					await transaction.RollbackAsync();
					return false;
				}
				// 確保角色存在
				await _userManager.AddToRoleAsync(user, "Vendor");


				var vendor = new Models.Vendors//建立廠商物件
				{
					UserId = user.Id,
					Account = newAccount,
					CompanyName = model.CompanyName,
					Email = model.Email,
					Status = true,//預設啟用，但廠商第一次登入後會先搜尋角色表有沒有缺漏資料才能用之後的功能
					CreateDate = DateTime.Now
				};

				_context.Vendors.Add(vendor);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();//成功後交易結束
				return true;
			}
			catch {
				await transaction.RollbackAsync();//失敗後交易回復
				throw;
			}

		}
	}
}
