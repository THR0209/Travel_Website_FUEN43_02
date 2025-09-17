using System.Threading.Tasks;
using Cat_Paw_Footprint.Areas.Order.Services;
using Cat_Paw_Footprint.Areas.Order.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cat_Paw_Footprint.Areas.Order.Controllers
{
	[Area("Order")]
	public class MailController : Controller
	{
		private readonly IEmailSender _sender;
		public MailController(IEmailSender sender) => _sender = sender;

		[HttpGet]
		public IActionResult Compose(int? orderId, string? to)
		{
			var vm = new MailComposeVm
			{
				OrderId = orderId,
				To = to ?? "",
				Subject = orderId.HasValue ? $"您的訂單通知（Order #{orderId}）" : "通知信",
				Body = "<p>您好，這是通知內容。</p>"
			};
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Send(MailComposeVm vm)
		{
			if (!ModelState.IsValid) return View("Compose", vm);
			await _sender.SendAsync(vm.To, vm.Subject, vm.Body);
			TempData["MailOk"] = "郵件已送出。";
			return RedirectToAction("Index", "CustomerOrders", new { area = "Order" });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SendAjax([FromForm] string to, [FromForm] string subject, [FromForm] string body)
		{
			if (string.IsNullOrWhiteSpace(to))
				return BadRequest(new { ok = false, error = "收件者信箱為必填" });

			try
			{
				await _sender.SendAsync(to, subject ?? "(無主旨)", body ?? "");
				return Ok(new { ok = true });
			}
			catch (System.Exception ex)
			{
				return BadRequest(new { ok = false, error = ex.Message });
			}
		}
	}

	public class MailComposeVm
	{
		public int? OrderId { get; set; }
		public string To { get; set; } = "";
		public string Subject { get; set; } = "";
		public string Body { get; set; } = "";
	}
}
