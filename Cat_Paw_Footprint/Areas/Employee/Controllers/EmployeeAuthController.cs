using Cat_Paw_Footprint.Areas.Employee.Services;
using Cat_Paw_Footprint.Areas.Employee.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Claims;

namespace Cat_Paw_Footprint.Areas.Employee.Controllers
{
	[Area("Employee")]
	public class EmployeeAuthController: Controller//這邊搞登入與註冊功能
	{
		private readonly EmployeeDbContext _context;
		private readonly IEmployeeService _svc;

		public EmployeeAuthController(EmployeeDbContext context, IEmployeeService svc)
		{
			_context = context;
			_svc = svc;
		}
		#region 登入註冊基礎功能邏輯一次放這就好不傳到Service
		// 登入
		[HttpGet]
		public IActionResult Login()
		{
			var model = new LoginViewModel(); // ✅ 傳入空模型
			return View(model);
		}
		// 登入
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login([Bind("Account,Password")] LoginViewModel vm)
		
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

			var claims = new List<Claim>
	{
		new Claim("EmployeeID", emp.EmployeeID.ToString()),
		new Claim("EmployeeName", empName),
		new Claim("RoleID", emp.RoleID.ToString()),
		new Claim("RoleName", roleName),
		new Claim("Status", emp.Status.ToString()),
		new Claim(ClaimTypes.Name, emp.Account),
	};
			var identity = new ClaimsIdentity(claims, "EmployeeAuth");
			var principal = new ClaimsPrincipal(identity);
			await HttpContext.SignInAsync("EmployeeAuth", principal);

			return RedirectToAction("Index", "Home", new { area = "" });
		}

		// 產生角色下拉選單
		private void PopulateRoleList()
		{
			var roles = _context.EmployeeRoles
				.Where(r => r.RoleName != "superadmin")
				.ToList();

			ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName");
		}
		[HttpGet]// 註冊
		public IActionResult Register()
		{
			PopulateRoleList(); // GET 呼叫
			return View(new RegisterViewModel());
		}
		// 註冊
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
			// 產生員工代號
			var newEmpCode = _svc.GetNewEmployeeCodeAsync().Result;//預存程序產生的字串
			var emp = new Cat_Paw_Footprint.Models.Employees//註冊員工帳號
			{
				EmployeeCode=newEmpCode,//員工代號
				Account =model.Account,
				Password=BCrypt.Net.BCrypt.HashPassword(model.Password),
				RoleID=model.RoleId,
				CreateDate=DateTime.Now,
				Status= true,
				EmployeeProfile = new EmployeeProfile
				{
					EmployeeName = model.EmployeeName
				}
			};
			_context.Employees.Add(emp);
	
			_context.SaveChanges();


			return RedirectToAction("Privacy", "Home", new { area = "" });
		}
		// 登出
		[HttpPost]
		[ValidateAntiForgeryToken]
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
			return RedirectToAction("Index", "Home", new { area = "" });
		}
		#endregion
		#region 這邊開始用service
		[HttpGet]
		public async Task<IActionResult> EmployeeList()
		{
			var employees = await _svc.GetAllAsync();
			ViewBag.Roles = _context.EmployeeRoles
		.Select(r => new SelectListItem
		{
			Value = r.RoleID.ToString(),
			Text = r.RoleName
		})
		.ToList();
			return View(employees);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateRow(int id, bool status, string? password, int roleId)
		{
			var idStr = HttpContext.Session.GetString("EmpId");
			try
			{
				await _svc.UpdateAccountAsync(id, status, password, roleId, idStr);
				return Json(new { ok = true, message = "更新成功" });
			}
			catch (ArgumentException ex)
			{
				return Json(new { ok = false, message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return Json(new { ok = false, message = ex.Message });
			}
			catch (Exception)
			{
				return Json(new { ok = false, message = "發生未知錯誤" });
			}
		}
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> UpdateRow(int id, bool status, string? password, int roleId)
		//{
		//	var result = await _svc.UpdateAccountAsync(id, status, password, roleId);
		//	return Json(result);
		//}
		[HttpGet]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EmployeeListNotUpdate()//彈出子視窗用但不確定是不是這樣寫
		{
			var employees = await _svc.GetAllAsync();
			ViewBag.Roles = _context.EmployeeRoles
		.Select(r => new SelectListItem
		{
			Value = r.RoleID.ToString(),
			Text = r.RoleName
		})
		.ToList();
			return View(employees);
		}

		[HttpGet]
		public async Task<IActionResult> Detail(int id)
		{
			var vm = await _svc.GetByIdAsync(id);
			if (vm == null) return NotFound();
			return PartialView("_EmployeeDetail", vm);
		}

		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			var idStr = HttpContext.Session.GetString("EmpId");
			if (!int.TryParse(idStr, out var empId))
				return RedirectToAction("Login", "EmployeeAuth", new { area = "Employee" });

			var vm = await _svc.GetByIdAsync(empId);
			if (vm == null) return NotFound();

			// 轉換成 ProfileEditInput
			var input = new ProfileEditInput
			{
				EmployeeName = vm!.EmployeeName!,
				Phone = vm.Phone,
				Email = vm.Email,
				IDNumber = vm.IDNumber,
				Address = vm.Address,
				ExistingPhoto = vm.Photo
			};

			return View("Profile", input);
		}

		[HttpPost]//單獨帳號個人資料頁更新
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Profile(ProfileEditInput input)
		{
			var empId = int.Parse(HttpContext.Session.GetString("EmpId")!);

			// 將檔案轉換成 byte[]
			byte[]? photoBytes = null;
			if (input.PhotoFile != null)
			{
				using var ms = new MemoryStream();
				await input.PhotoFile.CopyToAsync(ms);
				photoBytes = ms.ToArray();
			}

			var success = await _svc.UpdateSelfAsync(
				empId,
				input.EmployeeName,
				input.Phone,
				input.Email,
				input.Address,
				photoBytes,
				input.NewPassword,
				input.IDNumber
			);

			if (success) {
				if (success && !string.IsNullOrEmpty(input.NewPassword))
				{ // 1) 清 Session
					HttpContext.Session.Clear();

					// 2) 刪 Session Cookie（下次發新 SessionId，防 fixation）
					Response.Cookies.Delete(".AspNetCore.Session");
					await HttpContext.SignOutAsync("EmployeeAuth");

					// 3) 如果你有自訂 cookie 名稱也一併刪掉（有就留、沒有就刪掉這兩行）
					Response.Cookies.Delete(".CatPaw.Session");
					Response.Cookies.Delete(".CatPaw.Auth");

					// 4) 回登入頁
					return Json(new { ok = true, needRelogin = true, message = "密碼更新成功，請重新登入" });
				}


				return Json(new { ok = true, message = "更新成功" }); 
			}

			else
			{ return Json(new { ok = false, message = "更新失敗" }); }
		}

		#endregion
	}
}
