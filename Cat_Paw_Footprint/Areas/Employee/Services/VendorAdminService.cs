using Cat_Paw_Footprint.Areas.Employee.Repositories;
using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Microsoft.AspNetCore.Identity;
namespace Cat_Paw_Footprint.Areas.Employee.Services
{
	public class VendorAdminService : IVendorAdminService
	{
		private readonly IEmployeeRepository _repo;
		private readonly UserManager<IdentityUser> _userManager;      // 用 Identity 管帳號
		private readonly RoleManager<IdentityRole> _roleManager;      // 如果要加角色

		public VendorAdminService(IEmployeeRepository repo, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_repo = repo;
			_userManager = userManager;
			_roleManager = roleManager;
		}
	}
}
