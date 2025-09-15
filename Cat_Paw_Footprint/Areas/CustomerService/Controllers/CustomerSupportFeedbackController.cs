using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Microsoft.AspNetCore.Mvc;

[Area("CustomerService")]
[Route("CustomerService/[controller]")]
public class CustomerSupportFeedbackController : Controller
{
	private readonly ICustomerSupportFeedbackService _service;

	public CustomerSupportFeedbackController(ICustomerSupportFeedbackService service)
	{
		_service = service;
	}

	[HttpGet("")]
	public IActionResult Index() => View();

	[HttpGet("GetAll")]
	public async Task<IActionResult> GetAll()
	{
		var feedbacks = await _service.GetAllAsync();
		return Json(feedbacks.Select(f => new
		{
			f.FeedbackID,
			TicketID = f.Ticket?.TicketID,
			f.FeedbackRating,
			f.FeedbackComment,
			f.CreateTime
		}));
	}

	[HttpGet("Details/{id}")]
	public async Task<IActionResult> Details(int id)
	{
		var feedback = await _service.GetByIdAsync(id);
		if (feedback == null) return NotFound();

		// 只取必要欄位，避免循環
		var result = new
		{
			feedback.FeedbackID,
			TicketID = feedback.Ticket?.TicketID,
			feedback.FeedbackRating,
			feedback.FeedbackComment,
			feedback.CreateTime
		};

		return Json(result);
	}


	[HttpDelete("Delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		await _service.DeleteAsync(id);
		return Json(new { success = true });
	}
}
