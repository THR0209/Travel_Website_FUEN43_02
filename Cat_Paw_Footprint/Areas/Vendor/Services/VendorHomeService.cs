using Cat_Paw_Footprint.Areas.Vendor.Repositories;
using Cat_Paw_Footprint.Areas.Vendor.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace Cat_Paw_Footprint.Areas.Vendor.Services
{
	public class VendorHomeService: IVendorHomeService
	{
		private readonly IVendorHomeRepository _repo;
		private readonly UserManager<IdentityUser> _userManager;      // 用 Identity 管帳號
		private readonly RoleManager<IdentityRole> _roleManager;      // 如果要加角色
		public VendorHomeService(IVendorHomeRepository repo,
								UserManager<IdentityUser> userManager,
								RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_repo = repo;
		}
	}
}
