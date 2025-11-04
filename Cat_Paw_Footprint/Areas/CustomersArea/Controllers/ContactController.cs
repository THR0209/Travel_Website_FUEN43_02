using Cat_Paw_Footprint.Areas.Order.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	public class ContactController : Controller
	{
		private readonly IEmailSender _sender;

		public ContactController(IEmailSender sender)
		{
			_sender = sender;
		}

		[HttpGet]
		public IActionResult Index() => View(new ContactFormVm());

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Send([FromBody] ContactFormVm vm)
		{
			if (!ModelState.IsValid)
			{
				return Json(new { status = "error", message = "表單驗證失敗" });
			}

			try
			{
				// 🔹 寄給客服信箱
				string toService = "Ed5941234@gmail.com"; // 你的客服信箱
				string subject = $"[網站聯絡信] {vm.Subject}";
				string htmlBody = $@"
						<p><strong>寄件者姓名：</strong> {vm.Name}</p>
						<p><strong>Email：</strong> {vm.Email}</p>
						<p><strong>主旨：</strong> {vm.Subject}</p>
						<p><strong>訊息內容：</strong></p>
						<div style='white-space:pre-line'>{vm.Message}</div>
						<hr>
						<p style='font-size:12px;color:#888;'>此信件由「貓爪足跡」前台聯絡表單自動發出</p>";

				await _sender.SendAsync(toService, subject, htmlBody);

				// 🔹 自動回覆
				string autoSubject = "🐾 感謝您的來信 - 貓爪足跡客服中心";
				string autoBody = $@"
						<p>親愛的 {vm.Name} 您好：</p>
						<p>感謝您聯絡 <strong>貓爪足跡客服中心</strong>！</p>
						<p>我們已收到您的訊息：</p>
						<blockquote style='border-left:4px solid #7ec8e3;padding-left:10px;color:#555;'>
							{vm.Message.Replace("\n", "<br>")}
						</blockquote>
						<p>客服專員將於 1~2 個工作日內回覆您。</p>
						<p style='margin-top:1.5em'>祝您旅途愉快！<br>🐾 貓爪足跡 客服團隊</p>";

				await _sender.SendAsync(vm.Email, autoSubject, autoBody);

				return Json(new { status = "success" });
			}
			catch (Exception ex)
			{
				return Json(new { status = "error", message = ex.Message });
			}
		}

	}



	public class ContactFormVm
	{
		[Required(ErrorMessage = "請輸入姓名")]
		[Display(Name = "您的姓名")]
		public string Name { get; set; } = "";

		[Required(ErrorMessage = "請輸入Email")]
		[EmailAddress(ErrorMessage = "請輸入有效的Email")]
		[Display(Name = "您的Email")]
		public string Email { get; set; } = "";

		[Required(ErrorMessage = "請輸入主旨")]
		[Display(Name = "主旨")]
		public string Subject { get; set; } = "";

		[Required(ErrorMessage = "請輸入訊息內容")]
		[Display(Name = "訊息內容")]
		public string Message { get; set; } = "";
	}


}
