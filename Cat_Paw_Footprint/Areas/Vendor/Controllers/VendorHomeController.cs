using Cat_Paw_Footprint.Areas.Vendor.Services;
using Cat_Paw_Footprint.Areas.Vendor.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
namespace Cat_Paw_Footprint.Areas.Vendor.Controllers
{
	[Area("Vendor")]
	[Authorize(AuthenticationSchemes = "VendorAuth")]
	public class VendorHomeController : Controller
	{

		private readonly EmployeeDbContext _context;
		private readonly IVendorHomeService _svc;//處理廠商個資與登入邏輯
		public VendorHomeController(EmployeeDbContext context, IVendorHomeService svc)
		{
			_context = context;
			_svc = svc;
		}
		//廠商首頁
		public IActionResult Index()
		{
			return View();
		}
		//廠商登入頁面
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login()
		{
			await HttpContext.SignOutAsync("VendorAuth");//確保登出
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string account, string password)
		{
			var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";//取得使用者IP
			var vendor = await _svc.LoginAsync(account, password, ip);

			if (vendor.Message != "登入成功")
			{
				ModelState.AddModelError("", vendor.Message);
				return View();
			}

			return RedirectToAction("Index", "VendorHome", new { area = "Vendor" });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("VendorAuth");
			return RedirectToAction("Login", "VendorHome", new { area = "Vendor" });
		}
	}
}

