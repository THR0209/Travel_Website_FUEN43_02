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
	public class VendorAdminController: Controller
	{
		private readonly EmployeeDbContext _context;
		private readonly IVendorAdminService _svc;

		public VendorAdminController(EmployeeDbContext context, IVendorAdminService svc)//這粒概念是利用identity幫忙進行註冊 註冊時除了寫入identity的表單外 也會寫入我們自訂的資料表
		{
			_context = context;
			_svc = svc;
		}
		[HttpGet]
		public async Task<IActionResult> VendorRegister()//廠商註冊
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> VendorRegister(VendorAdminViewModel model)//廠商註冊
		{
			try
			{
				var success = await _svc.RegisterVendorAsync(model);
				if (success)
				{
					TempData["SuccessMessage"] = "廠商註冊成功，預設密碼為 Travel@123，請通知廠商登入後修改密碼。";
					return RedirectToAction(nameof(VendorList));
				}

				ModelState.AddModelError("", "廠商註冊失敗，請重試。");
			}
			catch (Exception ex)
			{
				// ❗ 這裡把 Service 丟回來的 Exception 轉成 ModelState 錯誤
				ModelState.AddModelError("", $"系統錯誤：{ex.Message}");
			}
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> VendorList()//廠商列表
		{
			var vendors = await _svc.GetAllVendorsAsync();  // 正確：解開 Task
			return View(vendors);
		}
		[HttpGet]
		public async Task<IActionResult> VendorDetail(VendorAdminViewModel model)//互動式按鈕取得單獨廠商資訊
		{
			var vendor = await _svc.GetVendorByAccountAsync(model.Account);
			if (vendor == null)
			{
				return NotFound(); // 或者回傳一個空的 ViewModel 避免 null
			}
			return PartialView("_VendorDetail", vendor);
		}
		[HttpGet]//互動式按鈕取得廠商登入紀錄
		public async Task<IActionResult> LoginHistory(VendorAdminViewModel model)
		{
			var loginHistory = await _svc.GetVendorsLoginHistoryAsync(model.VendorId);

			return PartialView("_LoginHistory", loginHistory);
		}
		[HttpPost]//更新按鈕
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateStatus(string account, bool status)
		{
			if (string.IsNullOrWhiteSpace(account))
			{
				return BadRequest("輸入錯誤");
			}
			var success = await _svc.UpdateStatusAsync(account, status);
			if (success)
			{
				return Json(new { success = true, message = "狀態更新成功" });
			}
			else
			{
				return Json(new { success = false, message = "狀態更新失敗" });
			}
		}
	}
}
