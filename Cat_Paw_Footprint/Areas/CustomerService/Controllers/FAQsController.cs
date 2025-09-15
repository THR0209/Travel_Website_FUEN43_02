using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Microsoft.AspNetCore.Mvc;
using static Cat_Paw_Footprint.Areas.CustomerService.ViewModel.FAQServiceDashboardViewModel;

namespace Cat_Paw_Footprint.Areas.CustomerService.Controllers
{
	/// <summary>
	/// FAQ 與 FAQ 分類管理 Controller，包含 Razor 頁面與 API
	/// </summary>
	[Area("CustomerService")]
	[Route("[area]/[controller]")]
	public class FAQsController : Controller
	{
		private readonly IFAQService _faqService;
		/// <summary>
		/// 注入 FAQ 服務層（Service）
		/// </summary>
		public FAQsController(IFAQService faqService) => _faqService = faqService;

		// ===== Razor View =====

		/// <summary>
		/// FAQ 維護主畫面
		/// </summary>
		[HttpGet("")]
		public IActionResult Index() => View("Index");

		// ===== FAQ API =====

		/// <summary>
		/// 取得所有 FAQ（API）
		/// </summary>
		[HttpGet("api")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var faqs = await _faqService.GetAllFAQsAsync();
				return Ok(faqs);// 回傳 FAQ 清單
			}
			catch (Exception ex)
			{
				// TODO: 建議換成正式 log
				Console.WriteLine(ex);
				return StatusCode(500, new { message = ex.Message });
			}
		}

		/// <summary>
		/// 取得單一 FAQ
		/// </summary>
		[HttpGet("api/{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var faq = await _faqService.GetFAQByIdAsync(id);
			return faq != null ? Ok(faq) : NotFound(new { message = "FAQ 不存在" });
		}

		/// <summary>
		/// 新增 FAQ
		/// </summary>
		[HttpPost("api")]
		public async Task<IActionResult> Create([FromBody] FAQViewModel faqVm)
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
		/// 更新 FAQ
		/// </summary>
		[HttpPut("api/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] FAQViewModel faqVm)
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
		/// 刪除 FAQ
		/// </summary>
		[HttpDelete("api/{id}")]
		public async Task<IActionResult> Delete(int id)
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

		// ===== FAQ Category API =====

		/// <summary>
		/// 取得所有分類
		/// </summary>
		[HttpGet("api/categories")]
		public async Task<IActionResult> GetCategories()
		{
			try
			{
				var cats = await _faqService.GetCategoriesAsync();
				// 過濾掉 CategoryID 或 CategoryName 為 null 的分類
				var result = cats
					.Where(c => c.CategoryID != null && c.CategoryName != null)
					.Select(c => new { id = c.CategoryID, name = c.CategoryName });
				return Ok(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex); // 建議 log ex.ToString()
				return StatusCode(500, new { message = ex.ToString() });
			}
		}

		/// <summary>
		/// 新增分類
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
		/// 修改分類
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
		/// 刪除 FAQ 分類（API，分類下有 FAQ 不能刪）
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
				// 若分類下仍有 FAQ，則回傳 400 BadRequest
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