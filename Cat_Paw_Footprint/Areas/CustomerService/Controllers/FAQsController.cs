using Cat_Paw_Footprint.Areas.CustomerService.Services; // 匯入 FAQ 服務介面
using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Authorization; // 匯入授權相關功能
using Microsoft.AspNetCore.Mvc; // 匯入 MVC 控制器相關功能
using Microsoft.AspNetCore.Mvc.Rendering; // 匯入 SelectList，用於下拉選單
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Cat_Paw_Footprint.Areas.CustomerService.ViewModel.FAQServiceDashboardViewModel;

namespace Cat_Paw_Footprint.Areas.CustomerService.Controllers
{
	/// <summary>
	/// FAQ 與 FAQ 分類管理 Controller，包含 Razor 頁面與 API
	/// </summary>
	[Area("CustomerService")] // 設定區域為 CustomerService
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaCustomerService")] // 設定僅授權員工存取
	[Route("[area]/[controller]")] // 路由格式：[區域]/[控制器]
	public class FAQsController : Controller
	{
		private readonly webtravel2Context _context;
		private readonly IFAQService _faqService; // FAQ 服務欄位

		// 建構式注入 FAQ 服務和 DbContext
		public FAQsController(IFAQService faqService, webtravel2Context context)
		{
			_faqService = faqService;
			_context = context;
		}

		public IActionResult TestDbColumns()
		{
			var connStr = _context.Database.GetDbConnection().ConnectionString; // 這裡要用 _context (不是 DbContext)
			using (var conn = new SqlConnection(connStr))
			{
				conn.Open();
				var cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FAQs'", conn);
				var reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					Console.WriteLine(reader.GetString(0));
				}
			}
			return Content("Done, check your console output.");
		}

		// ================= Razor View =================

		/// <summary>
		/// FAQ 管理主頁
		/// GET /CustomerService/FAQs
		/// </summary>
		[HttpGet("")]
		public IActionResult Index() => View("Index");

		/// <summary>
		/// 新增 FAQ 頁面
		/// GET /CustomerService/FAQs/Create
		/// </summary>
		[HttpGet("Create")]
		public async Task<IActionResult> Create()
		{
			var cats = await _faqService.GetCategoriesAsync(); // 取得 FAQ 分類
			ViewBag.CategoryID = new SelectList(cats, "CategoryID", "CategoryName");
			return View();
		}

		/// <summary>
		/// 新增 FAQ 表單送出
		/// POST /CustomerService/FAQs/Create
		/// </summary>
		[HttpPost("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(FAQViewModel faqVm)
		{
			if (!ModelState.IsValid)
			{
				// 表單驗證失敗，重取分類資料回填
				var cats = await _faqService.GetCategoriesAsync();
				ViewBag.CategoryID = new SelectList(cats, "CategoryID", "CategoryName", faqVm.CategoryID);
				return View(faqVm);
			}

			try
			{
				await _faqService.AddFAQAsync(faqVm); // 新增 FAQ
				TempData["SuccessMessage"] = "FAQ 已新增！";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				// 新增失敗，顯示錯誤訊息
				ModelState.AddModelError("", ex.Message);
				var cats = await _faqService.GetCategoriesAsync();
				ViewBag.CategoryID = new SelectList(cats, "CategoryID", "CategoryName", faqVm.CategoryID);
				return View(faqVm);
			}
		}

		/// <summary>
		/// 編輯 FAQ 頁面
		/// GET /CustomerService/FAQs/Edit/{id}
		/// </summary>
		[HttpGet("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var faq = await _faqService.GetFAQByIdAsync(id);
			if (faq == null)
			{
				TempData["ErrorMessage"] = "找不到該筆 FAQ。";
				return RedirectToAction("Index");
			}
			var cats = await _faqService.GetCategoriesAsync();
			ViewBag.CategoryID = new SelectList(cats, "CategoryID", "CategoryName", faq.CategoryID);
			return View(faq);
		}

		/// <summary>
		/// 編輯 FAQ 表單送出
		/// POST /CustomerService/FAQs/Edit/{id}
		/// </summary>
		[HttpPost("Edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, FAQViewModel faqVm)
		{
			if (id != faqVm.FAQID)
			{
				ModelState.AddModelError("", "ID 不一致");
			}

			if (!ModelState.IsValid)
			{
				// 表單驗證失敗，重取分類資料回填
				var cats = await _faqService.GetCategoriesAsync();
				ViewBag.CategoryID = new SelectList(cats, "CategoryID", "CategoryName", faqVm.CategoryID);
				return View(faqVm);
			}

			try
			{
				await _faqService.UpdateFAQAsync(faqVm); // 更新 FAQ
				TempData["SuccessMessage"] = "FAQ 已更新！";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				// 更新失敗，顯示錯誤訊息
				ModelState.AddModelError("", ex.Message);
				var cats = await _faqService.GetCategoriesAsync();
				ViewBag.CategoryID = new SelectList(cats, "CategoryID", "CategoryName", faqVm.CategoryID);
				return View(faqVm);
			}
		}

		// ================= FAQ API =================

		/// <summary>
		/// 取得所有 FAQ (API)
		/// GET /CustomerService/FAQs/api
		/// </summary>
		[HttpGet("api")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var faqs = await _faqService.GetAllFAQsAsync();
				return Ok(faqs);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return StatusCode(500, new { message = ex.Message });
			}
		}

		/// <summary>
		/// 取得單一 FAQ (API)
		/// GET /CustomerService/FAQs/api/{id}
		/// </summary>
		[HttpGet("api/{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var faq = await _faqService.GetFAQByIdAsync(id);
			return faq != null ? Ok(faq) : NotFound(new { message = "FAQ 不存在" });
		}

		/// <summary>
		/// 新增 FAQ (API)
		/// POST /CustomerService/FAQs/api
		/// </summary>
		[HttpPost("api")]
		public async Task<IActionResult> CreateApi([FromBody] FAQViewModel faqVm)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			try
			{
				await _faqService.AddFAQAsync(faqVm);
				return Ok(new { message = "FAQ 已新增!" });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, new { message = ex.Message });
			}
		}

		/// <summary>
		/// 更新 FAQ (API)
		/// PUT /CustomerService/FAQs/api/{id}
		/// </summary>
		[HttpPut("api/{id}")]
		public async Task<IActionResult> UpdateApi(int id, [FromBody] FAQViewModel faqVm)
		{
			if (id != faqVm.FAQID) return BadRequest(new { message = "ID 不一致" });
			try
			{
				await _faqService.UpdateFAQAsync(faqVm);
				return Ok(new { message = "FAQ 已更新!" });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, new { message = ex.Message });
			}
		}

		/// <summary>
		/// 刪除 FAQ (API)
		/// DELETE /CustomerService/FAQs/api/{id}
		/// </summary>
		[HttpDelete("api/{id}")]
		public async Task<IActionResult> DeleteApi(int id)
		{
			try
			{
				await _faqService.DeleteFAQAsync(id);
				return Ok(new { message = "FAQ 已刪除!" });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// ================= FAQ Category API =================

		/// <summary>
		/// 取得所有 FAQ 分類 (API)
		/// GET /CustomerService/FAQs/api/categories
		/// </summary>
		[HttpGet("api/categories")]
		public async Task<IActionResult> GetCategories()
		{
			try
			{
				var cats = await _faqService.GetCategoriesAsync();
				var result = cats
					.Where(c => c.CategoryID != null && c.CategoryName != null)
					.Select(c => new { id = c.CategoryID, name = c.CategoryName });
				return Ok(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, new { message = ex.ToString() });
			}
		}

		/// <summary>
		/// 新增 FAQ 分類 (API)
		/// POST /CustomerService/FAQs/api/categories
		/// </summary>
		[HttpPost("api/categories")]
		public async Task<IActionResult> CreateCategory([FromBody] FAQCategoryViewModel catVm)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			try
			{
				await _faqService.AddCategoryAsync(catVm);
				return Ok(new { message = "分類已新增!" });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, new { message = ex.Message });
			}
		}

		/// <summary>
		/// 更新 FAQ 分類 (API)
		/// PUT /CustomerService/FAQs/api/categories/{id}
		/// </summary>
		[HttpPut("api/categories/{id}")]
		public async Task<IActionResult> UpdateCategory(int id, [FromBody] FAQCategoryViewModel catVm)
		{
			if (id != catVm.CategoryID) return BadRequest(new { message = "ID 不一致" });
			try
			{
				await _faqService.UpdateCategoryAsync(catVm);
				return Ok(new { message = "分類已更新!" });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, new { message = ex.Message });
			}
		}

		/// <summary>
		/// 刪除 FAQ 分類 (API，分類下有 FAQ 不能刪)
		/// DELETE /CustomerService/FAQs/api/categories/{id}
		/// </summary>
		[HttpDelete("api/categories/{id}")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			try
			{
				await _faqService.DeleteCategoryAsync(id);
				return Ok(new { message = "分類已刪除!" });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, new { message = ex.Message });
			}
		}
	}
}