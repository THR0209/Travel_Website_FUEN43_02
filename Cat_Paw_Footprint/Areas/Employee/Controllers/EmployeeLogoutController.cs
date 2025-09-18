using Cat_Paw_Footprint.Areas.Employee.Services;
using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Claims;
namespace Cat_Paw_Footprint.Areas.Employee.Controllers
{
	[Area("Employee")]

	public class EmployeeLogoutController : Controller
	{
		private readonly EmployeeDbContext _context;
		private readonly IEmployeeService _svc;

		public EmployeeLogoutController(EmployeeDbContext context, IEmployeeService svc)
		{
			_context = context;
			_svc = svc;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> Logout()
		{
			// 1) 清 Session
			HttpContext.Session.Clear();

			// 2) 刪 Session Cookie（下次發新 SessionId，防 fixation）
			Response.Cookies.Delete(".AspNetCore.Session");
			await HttpContext.SignOutAsync("EmployeeAuth");

			// 3) 如果你有自訂 cookie 名稱也一併刪掉（有就留、沒有就刪掉這兩行）
			Response.Cookies.Delete(".CatPaw.Session");
			Response.Cookies.Delete(".CatPaw.Auth");

			// 4) 回登入頁
			return RedirectToAction("Login", "EmployeeAuth", new { area = "Employee" });
		}
	}
}
