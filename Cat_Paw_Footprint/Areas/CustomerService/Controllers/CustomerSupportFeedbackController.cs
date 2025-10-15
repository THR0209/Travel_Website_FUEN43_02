using Cat_Paw_Footprint.Areas.CustomerService.Services; // 匯入客戶服務領域的業務邏輯服務介面
using Microsoft.AspNetCore.Authorization; // 匯入身份驗證與授權相關功能
using Microsoft.AspNetCore.Mvc; // 匯入 MVC 控制器相關功能

// 設定此 Controller 屬於 CustomerService 區域
[Area("CustomerService")]
// 設定授權機制，僅限 EmployeeAuth 驗證且符合 AreaCustomerService 授權政策的使用者
[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaCustomerService")]
// 設定路由前綴為 CustomerService/[controller]，controller 會自動替換成 CustomerSupportFeedback
[Route("CustomerService/[controller]")]
public class CustomerSupportFeedbackController : Controller
{
	// 宣告私有欄位，用來儲存注入的客戶服務評價服務
	private readonly ICustomerSupportFeedbackService _service;

	// 透過建構式注入服務
	public CustomerSupportFeedbackController(ICustomerSupportFeedbackService service)
	{
		_service = service;
	}

	/// <summary>
	/// 顯示主頁面
	///  GET /CustomerService/CustomerSupportFeedback
	/// </summary>
	[HttpGet("")]
	public IActionResult Index() => View();

	/// <summary>
	/// 取得所有評價 (過濾掉工單狀態為待處理 StatusID == 1)
	///  GET /CustomerService/CustomerSupportFeedback/GetAll
	/// </summary>
	[HttpGet("GetAll")]
	public async Task<IActionResult> GetAll()
	{
		// 取得所有評價資料
		var feedbacks = await _service.GetAllAsync();

		// 過濾掉工單狀態為待處理 (StatusID == 1) 的評價
		var result = feedbacks
			.Where(f => f.Ticket != null && f.Ticket.StatusID != 1) // 確保工單存在且狀態不是待處理
			.Select(f => new
			{
				f.FeedbackID, // 評價 ID
				TicketID = f.Ticket?.TicketCode, // 工單編號
				f.FeedbackRating, // 評價分數
				f.FeedbackComment, // 評價內容
				f.CreateTime // 建立時間
			});

		return Json(result); // 以 JSON 格式回傳結果
	}

	/// <summary>
	/// 查詢特定評價詳細資料
	///  GET /CustomerService/CustomerSupportFeedback/Details/{id}
	/// </summary>
	[HttpGet("Details/{id}")]
	public async Task<IActionResult> Details(int id)
	{
		// 依評價 ID 查詢單筆資料
		var feedback = await _service.GetByIdAsync(id);
		if (feedback == null) return NotFound(); // 若無資料則回傳 404

		// 如果工單狀態為待處理 (StatusID == 1)，直接回傳 404
		if (feedback.Ticket?.StatusID == 1) return NotFound();

		// 組合要回傳的詳細資料
		var result = new
		{
			feedback.FeedbackID, // 評價 ID
			TicketID = feedback.Ticket?.TicketCode, // 工單編號
			feedback.FeedbackRating, // 評價分數
			feedback.FeedbackComment, // 評價內容
			feedback.CreateTime // 建立時間
		};

		return Json(result); // 以 JSON 格式回傳結果
	}

	/// <summary>
	/// 刪除特定評價
	///  DELETE /CustomerService/CustomerSupportFeedback/Delete/{id}
	/// </summary>
	[HttpDelete("Delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		await _service.DeleteAsync(id); // 執行刪除
		return Json(new { success = true }); // 回傳成功訊息
	}
}