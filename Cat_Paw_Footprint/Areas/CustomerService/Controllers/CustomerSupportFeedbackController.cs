using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Area("CustomerService")]
[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaCustomerService")]
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

		// 過濾掉工單狀態為待處理 (StatusID == 1) 的評價
		var result = feedbacks
			.Where(f => f.Ticket != null && f.Ticket.StatusID != 1)
			.Select(f => new
			{
				f.FeedbackID,
				TicketID = f.Ticket?.TicketCode, // 顯示工單編號
				f.FeedbackRating,
				f.FeedbackComment,
				f.CreateTime
			});

		return Json(result);
	}

	[HttpGet("Details/{id}")]
	public async Task<IActionResult> Details(int id)
	{
		var feedback = await _service.GetByIdAsync(id);
		if (feedback == null) return NotFound();

		// 如果工單是待處理，直接回 NotFound
		if (feedback.Ticket?.StatusID == 1) return NotFound();

		var result = new
		{
			feedback.FeedbackID,
			TicketID = feedback.Ticket?.TicketCode,
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
