using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;

namespace Cat_Paw_Footprint.Areas.Employee.Controllers
{
	[Area("Employee")]
	public class EmployeeAuthController: Controller//這邊搞登入與註冊功能
	{
		private readonly EmployeeDbContext _context;
		public EmployeeAuthController(EmployeeDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Login()
		{
			var model = new LoginViewModel(); // ✅ 傳入空模型
			return View(model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login([Bind("Account,Password")] LoginViewModel vm)
		
		{
			if (string.IsNullOrWhiteSpace(vm.Account))
				ModelState.AddModelError(nameof(vm.Account), "請輸入帳號");
			if (string.IsNullOrWhiteSpace(vm.Password))
				ModelState.AddModelError(nameof(vm.Password), "請輸入密碼");
			if (!ModelState.IsValid)
			{
				return View(vm);
			}
			// ❗ 改成只用帳號找，先不要比對密碼
			var emp = _context.Employees.FirstOrDefault(e => e.Account == vm.Account);
			
			// ❗ 沒找到帳號
			if (emp == null)
			{
				Console.WriteLine("登入失敗：帳號不存在 " + vm.Account);
				vm.ErrorMessage = "帳號不存在";
				return View(vm);
			}
			System.Diagnostics.Debug.WriteLine("🧪 讀取帳號資訊");
			System.Diagnostics.Debug.WriteLine($"資料庫內部帳號:{emp.Account}");
			System.Diagnostics.Debug.WriteLine($"資料庫內部密碼:{emp.Password}");
			System.Diagnostics.Debug.WriteLine("🧪 讀取完畢");

			// ❗ 密碼比對失敗
			if (!BCrypt.Net.BCrypt.Verify(vm.Password, emp.Password))
			{
				vm.ErrorMessage = "密碼錯誤";
				return View(vm);
			}
			System.Diagnostics.Debug.WriteLine("🧪 帳號密碼比對成功");
			var profile = _context.EmployeeProfile
			.FirstOrDefault(p => p.EmployeeID == emp.EmployeeID);

			string empName = profile?.EmployeeName ?? "未填寫";

			var roleName = _context.EmployeeRoles
				.Where(r => r.RoleID == emp.RoleID)
				.Select(r => r.RoleName)
				.FirstOrDefault() ?? string.Empty;


			if (emp.Status!= true)
			{
				vm.ErrorMessage = "帳號被停用，請聯絡管理員";
				return View(vm);
			}// ❗ 帳號被停用


			HttpContext.Session.SetString("EmpId", emp.EmployeeID.ToString());
			HttpContext.Session.SetString("EmpRoleId", emp.RoleID.ToString());
			HttpContext.Session.SetString("EmpRoleName", roleName);
			HttpContext.Session.SetString("EmpName", empName);
			HttpContext.Session.SetString("Status", emp.Status.ToString());
			HttpContext.Session.SetString("Login", "True");
			return RedirectToAction("Index", "Home", new { area = "" });
		}


		private void PopulateRoleList()
		{
			var roles = _context.EmployeeRoles
				.Select(r => new { r.RoleID, r.RoleName })
				.ToList();

			ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName");
		}
		[HttpGet]
		public IActionResult Register()
		{
			PopulateRoleList(); // GET 呼叫
			return View(new RegisterViewModel());
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Register(RegisterViewModel model)
		{
			PopulateRoleList();
			
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			if (_context.Employees.Any(e => e.Account == model.Account))
			{
				ModelState.AddModelError(nameof(model.Account), "此帳號已被註冊");
				
				return View(model);
			}
			var emp = new Cat_Paw_Footprint.Models.Employees//註冊員工帳號
			{
				Account=model.Account,
				Password=BCrypt.Net.BCrypt.HashPassword(model.Password),
				RoleID=model.RoleId,
				CreateDate=DateTime.Now,
				Status= true
			};
			_context.Employees.Add(emp);
			_context.SaveChanges();

			var profile = new EmployeeProfile//註冊之後產生基本員工個資表，剩下給員工自己寫
			{
				EmployeeID=emp.EmployeeID,
				EmployeeName=model.EmployeeName,
			};

			_context.EmployeeProfile.Add(profile);
			_context.SaveChanges();


			return RedirectToAction("Privacy", "Home", new { area = "" });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Logout()
		{
			// 1) 清 Session
			HttpContext.Session.Clear();

			// 2) 刪 Session Cookie（下次發新 SessionId，防 fixation）
			Response.Cookies.Delete(".AspNetCore.Session");

			// 3) 如果你有自訂 cookie 名稱也一併刪掉（有就留、沒有就刪掉這兩行）
			Response.Cookies.Delete(".CatPaw.Session");
			Response.Cookies.Delete(".CatPaw.Auth");

			// 4) 回登入頁
			return RedirectToAction("Index", "Home", new { area = "" });
		}
	}
}
