using Cat_Paw_Footprint.Areas.Vendor.Repositories;
using Cat_Paw_Footprint.Areas.Vendor.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Cat_Paw_Footprint.Areas.Vendor.Services
{
	public class VendorHomeService: IVendorHomeService
	{
		private readonly IVendorHomeRepository _repo;
		private readonly UserManager<IdentityUser> _userManager;      // 用 Identity 管帳號
		private readonly RoleManager<IdentityRole> _roleManager;      // 如果要加角色
		private readonly SignInManager<IdentityUser> _signInManager;// 用 Identity 管登入登出
		private readonly IHttpContextAccessor _httpContextAccessor;
		public VendorHomeService(IVendorHomeRepository repo,
								UserManager<IdentityUser> userManager,
								RoleManager<IdentityRole> roleManager, 
								SignInManager<IdentityUser> signInManager,
								IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_repo = repo;
			_signInManager = signInManager;
			_httpContextAccessor = httpContextAccessor;
		}
		//廠商登入
		public async Task<VendorHomeViewModel> LoginAsync(string account, string password, string ip)
		{
			var vendor = await _repo.GetVendorByAccountAsync(account);
			if (vendor == null)
			{
				// 帳號不存在或被停用
				return new VendorHomeViewModel
				{
					Message = "帳號不存在"
				};
			}
			if (vendor.Status==false)
			{
				// 帳號不存在或被停用
				return new VendorHomeViewModel
				{
					Message = "帳號已被停用，請聯絡管理員"
				};
			}
			var user = await _userManager.FindByNameAsync(account);
			if (user == null)
			{
				return new VendorHomeViewModel
				{
					Message = "帳號已遺失，請聯絡管理員"
				};
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
			if (!result.Succeeded)
			{
				// 建立錯誤登入紀錄
				await _repo.CreateLoginLogAsync(vendor.VendorId, false, ip);

				// 判斷最近三次是否全失敗 → 鎖帳
				var last3 = await _repo.GetLast3LoginLogsAsync(vendor.VendorId);
				if (last3.Count() >= 3 && last3.All(l => l.IsSuccessful == false))
				{
					await _repo.LockVendorAccountAsync(vendor.VendorId);
					return new VendorHomeViewModel { Message = "密碼錯誤超過 3 次，帳號已被鎖定，解除綁定麻煩找管理員" };
				}

				return new VendorHomeViewModel { Message = "密碼錯誤" };
			}

			// 4. 成功 → 建立登入紀錄 & 更新最後登入時間
			var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

			await _httpContextAccessor.HttpContext.SignInAsync(
				"VendorAuth",   // 你的 VendorAuth Scheme
				claimsPrincipal,
				new AuthenticationProperties
				{
					IsPersistent = false,
					ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
				});
			await _repo.CreateLoginLogAsync(vendor.VendorId, true, ip);
			await _repo.UpdateLastLoginAsync(vendor.VendorId, DateTime.Now, ip);

			vendor.Message = "登入成功";
			return vendor;

		}
	}
}
