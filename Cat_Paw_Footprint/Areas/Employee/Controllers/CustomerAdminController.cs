using Cat_Paw_Footprint.Areas.Employee.Services;
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
	public class CustomerAdminController : Controller
	{
		private readonly EmployeeDbContext _context;
		private readonly ICustomerAdminService _svc;
		public CustomerAdminController(EmployeeDbContext context, ICustomerAdminService svc)
		{
			_context = context;
			_svc = svc;
		}

		[HttpGet]
		public async Task<IActionResult> CustomerList()
		{
			var customers = await _svc.GetAllCustomersAsync();  // 正確：解開 Task
			ViewBag.Levels = _context.CustomerLevels.Select(r => new SelectListItem
			{
				Value = r.Level.ToString(),
				Text = r.LevelName
			}).ToList();

			return View(customers);
		}

		[HttpGet]
		public async Task<IActionResult> CustomerDetail(string email)//互動式按鈕
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				return BadRequest("輸入錯誤");
			}

			var customerId = await _svc.GetCustomerIdByEmailAsync(email);
			if (customerId == null)
			{
				return NotFound("查無此帳戶");
			}

			var customer = await _svc.GetCustomerByIdAsync(customerId);
			return PartialView("_CustomerDetail", customer);
		}
	
	[HttpGet]//登入紀錄
		public async Task<IActionResult> LoginHistory(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				return BadRequest("輸入錯誤");
			}
			var customerId = await _svc.GetCustomerIdByEmailAsync(email);
			if (customerId == null)
			{
				return NotFound("查無此帳戶");
			}
			var loginHistory = await _svc.GetLoginHistoryAsync(customerId);
			return PartialView("_LoginHistory", loginHistory);
		}
		[HttpPost]//儲存按鈕
		public async Task<IActionResult> UpdateCustomer(int customerId, int levelId, bool status, bool isBlacklisted)
		{
			if (customerId <= 0)
			{
				return BadRequest("輸入錯誤");
			}
			var updateLevelResult = await _svc.UpdateLevelAsync(customerId, levelId);
			var updateBlacklistResult = await _svc.UpdateBlacklistStatusAsync(customerId, isBlacklisted);
			var updateStatusResult = await _svc.UpdateStatusAsync(customerId, status);
			if (updateLevelResult && updateBlacklistResult && updateStatusResult)
			{
				return Json(new { ok = true, message = "更新成功" });
			}
			else
			{
				return Json(new { ok = false, message = "更新失敗" });
			}
		}

	} 
}
