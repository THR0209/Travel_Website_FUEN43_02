using Cat_Paw_Footprint.Areas.CustomersArea.Services;
using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Authorize(AuthenticationSchemes = "CustomerAuth")]
	public class CusLogRegController : Controller//登入註冊修改資料
	{
		private readonly ApplicationDbContext _context;
		private readonly ICusLogRegService _svc;//處理客戶個資與登入邏輯
        private readonly MemberLevelService _memberLevelService;

        public CusLogRegController(ApplicationDbContext context, ICusLogRegService svc,
        MemberLevelService memberLevelService)
		{
			_context = context;
			_svc = svc;
            _memberLevelService = memberLevelService;
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

			// 核心修正：設置 Session，讓API可以正確讀取
			HttpContext.Session.SetInt32("CustomerID", customer.CustomerId);
			HttpContext.Session.SetString("CustomerName", customer.FullName ?? customer.Account ?? "");

			return Ok(new { success = true, message = "登入成功", redirectUrl = "/CustomersArea/Home/Index" });
		}
		[HttpPost]
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
			//如果註冊成功 發放新客戶優惠券
			// ✅ 發放新客戶優惠券
			var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Account == account);
            try
            {
                if (customer != null)
                    await _memberLevelService.GrantCouponsForTypeAsync(customer.CustomerID, "Register");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發放優惠券時發生錯誤: {ex.Message}");
            }



            return Ok(new { success = true, message = "註冊成功", redirectUrl = "/CustomersArea/CusLogReg/Login" });
		}
		//客戶修改資料介面
		[Area("CustomersArea")]
		[Authorize(AuthenticationSchemes = "CustomerAuth")]
		[HttpGet]
		public async Task<IActionResult> GetProfile()
		{
			var claim = User.Claims.FirstOrDefault(c => c.Type == "CustomerId");
			if (claim == null)
				return Unauthorized(new { success = false, error = "未登入" });

			int id = int.Parse(claim.Value);
			var customer = await _context.Customers
				.Include(c => c.CustomerProfile)
				.FirstOrDefaultAsync(c => c.CustomerID == id);

			if (customer == null)
				return NotFound(new { success = false, error = "找不到客戶資料" });

			return Ok(new
			{
				success = true,
				data = new
				{
					fullName = customer.CustomerProfile.CustomerName,
					phone = customer.CustomerProfile.Phone,
					address = customer.CustomerProfile.Address,
					idNumber = customer.CustomerProfile.IDNumber,
					email = customer.CustomerProfile.Email
				}
			});
		}
		[HttpGet]
		public IActionResult EditProfile()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditProfile([FromBody] CusLogRegDto dto)
		{
			var customerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "CustomerId");
			if (customerIdClaim == null)
				return BadRequest(new { success = false, error = "無法取得使用者資料，請重新登入" });

			dto.CustomerId = int.Parse(customerIdClaim.Value);
			var updatedCustomer = await _svc.UpdateCustomerAsync(dto);
			if (!string.IsNullOrEmpty(updatedCustomer?.ErrorMessage))
				return BadRequest(new { success = false, error = updatedCustomer.ErrorMessage });

			return Ok(new { success = true, message = updatedCustomer.Message });
		}
		//客戶修改密碼介面
		[HttpGet]
		public IActionResult ChangePassword()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword([FromBody] CusLogRegDto dto)// 客戶修改密碼
		{
			var newPassword = dto.Password;
			var email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
			var updatedCustomer = await _svc.UpdateCustomerPasswordAsync(email, newPassword);
			if (!string.IsNullOrEmpty(updatedCustomer?.ErrorMessage))
			{
				return BadRequest(new { success = false, error = updatedCustomer.ErrorMessage });
			}
			return Ok(new { success = true, message = updatedCustomer.Message });
		}
		[HttpGet]//忘記密碼介面
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword([FromBody] CusLogRegDto dto)// 忘記密碼
		{
			var email = dto.Email;
			var result = await _svc.EmailToUser(email);
			if (!string.IsNullOrEmpty(result?.ErrorMessage))
			{
				return BadRequest(new { success = false, error = result.ErrorMessage });
			}
			return Ok(new { success = true, message = "如果該 Email 存在，我們已發送重設密碼的指示。" });
		}
		[HttpGet]
		[AllowAnonymous]//重設密碼介面(登入後介面)
		public IActionResult ResetPasswordIslogin()
		{
			return View();
		}
		[HttpGet]
		[AllowAnonymous]//重設密碼介面(忘記密碼後介面)
		public IActionResult ResetPassword()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> ResetPassword([FromBody] CusLogRegDto dto)// 重設密碼
		{
			string email = dto.Email;
			string newPassword = dto.Password;
			if (email == null)
			{
				email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
			}
			var updatedCustomer = await _svc.UpdateCustomerPasswordAsync(email, newPassword);
			if (!string.IsNullOrEmpty(updatedCustomer?.ErrorMessage))
			{
				return BadRequest(new { success = false, error = updatedCustomer.ErrorMessage });
			}

			await HttpContext.SignOutAsync("CustomerAuth");
			return Ok(new { success = true, message = updatedCustomer.Message });
		}



	}
}