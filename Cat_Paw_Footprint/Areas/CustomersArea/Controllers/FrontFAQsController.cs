using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[AllowAnonymous]
	[Route("CustomersArea/FrontFAQs")]
	public class FrontFAQsController : Controller
	{
		private readonly IFAQService _faqService;
		public FrontFAQsController(IFAQService faqService)
		{
			_faqService = faqService;
		}

		//GET: CustomersArea/FrontFAQs/Hot
		[HttpGet]
		[Route("api/hot")]
		public async Task<IActionResult> Hot(int count = 5)
		{
			var hotFaqs = await _faqService.GetHotFAQsAsync(count);
			return Ok(hotFaqs);
		}

		//GET: CustomersArea/FrontFAQs
		[HttpGet]
		[Route("Index")]
		public IActionResult Index()
		{
			return View();
		}

		//GET: CustomersArea/FrontFAQs/All
		[HttpGet]
		[Route("api/all")]
		public async Task<IActionResult> All()
		{
			var faqs = await _faqService.GetAllFAQsAsync();
			return Ok(faqs.Where(f => f.IsActive).ToList());
		}

		//GET: CustomersArea/FrontFAQs/Categories
		[HttpGet]
		[Route("api/categories")]
		public async Task<IActionResult> Categories()
		{
			var cats = await _faqService.GetCategoriesAsync();
			var result = cats
				.Where(c => c.CategoryID != null && c.CategoryName != null)
				.Select(c => new { id = c.CategoryID, name = c.CategoryName });
			return Ok(result);
		}
	}
}