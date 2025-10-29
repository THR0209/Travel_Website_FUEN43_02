using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Areas.Notification.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[Authorize(AuthenticationSchemes = "CustomerAuth")]
	[Route("CustomersArea/[controller]/[action]")]
	public class NotificationsController : Controller
	{
		private readonly INotificationService _notificationService;
		private int CurrentCustomerId =>
			int.TryParse(User.FindFirst("CustomerID")?.Value, out var id) ? id : 0;

		public NotificationsController(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}

		/// <summary>通知中心主頁</summary>
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var notifications = await _notificationService.GetUserNotificationsAsync(CurrentCustomerId);
			var vmList = notifications.Select(n => new NotificationViewModel
			{
				NotificationID = n.NotificationID,
				Title = n.Title,
				Message = n.Message,
				Type = n.Type,
				IsRead = n.IsRead,
				CreatedAt = n.CreatedAt,
				ReadAt = n.ReadAt
			}).OrderByDescending(n => n.CreatedAt).ToList();

			return View(vmList);
		}

		/// <summary>標示單筆為已讀</summary>
		[HttpPost]
		public async Task<IActionResult> MarkAsRead([FromBody] MarkReadRequest request)
		{
			try
			{
				if (!User.Identity?.IsAuthenticated ?? true)
					return Unauthorized(new { success = false, message = "未登入" });

				if (request == null || request.Id <= 0)
					return BadRequest(new { success = false, message = "通知 ID 無效" });

				await _notificationService.MarkAsReadAsync(request.Id);
				return Ok(new { success = true });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ MarkAsRead 發生例外：{ex.Message}\n{ex.StackTrace}");
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>未讀通知數量（給 Layout 用）</summary>
		[HttpGet]
		public async Task<IActionResult> GetUnreadCount()
		{
			var list = await _notificationService.GetUserNotificationsAsync(CurrentCustomerId);
			var count = list.Count(n => !n.IsRead);
			return Json(new { count });
		}

		/// <summary>最新通知 3 筆（給 Layout 用）</summary>
		[HttpGet]
		public async Task<IActionResult> GetLatestNotifications()
		{
			var list = (await _notificationService.GetUserNotificationsAsync(CurrentCustomerId))
				.OrderByDescending(n => n.CreatedAt)
				.Take(3)
				.Select(n => new
				{
					n.NotificationID,
					n.Title,
					n.Message,
					n.Type,
					n.IsRead,
					n.CreatedAt
				});
			return Json(list);
		}
	}

	public class MarkReadRequest
	{
		public int Id { get; set; }
	}
}
