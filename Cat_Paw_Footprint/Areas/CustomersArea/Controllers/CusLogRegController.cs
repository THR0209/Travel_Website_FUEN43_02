using Cat_Paw_Footprint.Areas.CustomersArea.Services;
using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Authorize(AuthenticationSchemes = "CustomerAuth")]
	public class CusLogRegController: Controller//登入註冊修改資料
	{
		private readonly ApplicationDbContext _context;
		private readonly ICusLogRegService _svc;//處理客戶個資與登入邏輯

		public CusLogRegController(ApplicationDbContext context, ICusLogRegService svc)
		{
			_context = context;
			_svc = svc;
		}
		//客戶首頁
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index()
		{
			return View();
		}
		//客戶登入介面
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string account, string password)
		{           // 登入邏輯待實作

			var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";//取得使用者IP
			var customer = await _svc.LoginAsync(account, password, ip);
			if (customer.Message != "登入成功")
			{
				return BadRequest(new { success = false, error = customer.ErrorMessage });
			}
			//測試用
			Console.WriteLine(customer.CustomerId);
			Console.WriteLine(customer.UserId);
			Console.WriteLine(customer.Account);
			Console.WriteLine(customer.FullName);
			Console.WriteLine(customer.Email);
			Console.WriteLine(customer.Phone);
			Console.WriteLine(customer.Address);
			Console.WriteLine(customer.Level);
			Console.WriteLine(customer.LevelName);

			var claims = new List<Claim>// 建立 Claims
			{
				new Claim("CustomerId", customer.CustomerId.ToString()),
				new Claim("UserId", customer.UserId ?? ""),
				new Claim("Account", customer.Account ?? ""),
				new Claim("FullName", customer.FullName ?? ""),
				new Claim("Email", customer.Email ?? ""),
				new Claim("Phone", customer.Phone ?? ""),
				new Claim("Address", customer.Address ?? ""),
				new Claim("Level", customer.Level?.ToString() ?? ""),
				new Claim("LevelName", customer.LevelName ?? "")
			};

			var claimsIdentity = new ClaimsIdentity(claims, "CustomerAuth");// 建立 ClaimsIdentity

			await HttpContext.SignInAsync(
				"CustomerAuth",
				new ClaimsPrincipal(claimsIdentity),
				new AuthenticationProperties
				{
					IsPersistent = true,
					ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
				}
			);

			return Ok(new { success = true, message = "登入成功", redirectUrl = "/CustomersArea/CusLogReg/Index" });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("CustomerAuth");
			return RedirectToAction("Login", "CusLogReg", new { area = "CustomersArea" });
		}
		//客戶註冊介面
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register()
		{
			return View();
		}
		//客戶註冊
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> Register(string account, string password, string fullName, string email, string phone, string address)
		{

			var dto = new CusLogRegDto
			{
				Account = account,
				Password = password,
				FullName = fullName,
				Email = email,
				Phone = phone,
				Address = address
			};
			var result = await _svc.RegisterCustomerAsync(dto);
			if (result != "註冊成功")
			{
				return BadRequest(new { success = false, error = result });
			}
			return Ok(new { success = true, message = "註冊成功", redirectUrl = "/CustomersArea/CusLogReg/Login" });
		}
		//客戶修改資料介面
		[HttpGet]
		public IActionResult EditProfile()
		{
			return View();
		}
		//客戶修改密碼介面
		[HttpGet]
		public IActionResult ChangePassword()
		{
			return View();
		}

	}
}
