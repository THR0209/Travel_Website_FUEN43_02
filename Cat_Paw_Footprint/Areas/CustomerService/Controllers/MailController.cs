using Cat_Paw_Footprint.Areas.Order.Services; // 匯入寄信服務介面
using Microsoft.AspNetCore.Authorization; // 匯入授權相關功能
using Microsoft.AspNetCore.Mvc; // 匯入 MVC 控制器相關功能
using System.Collections.Generic;
using System.Linq;

namespace Cat_Paw_Footprint.Areas.CustomerService.Controllers
{
	[Area("CustomerService")] // 設定區域為 CustomerService
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaCustomerService")] // 限員工授權存取
	public class MailController : Controller
	{
		private readonly IEmailSender _sender; // 寄信服務欄位

		// 建構式注入寄信服務
		public MailController(IEmailSender sender)
		{
			_sender = sender;
		}
		
		// 郵件範本資料 (可改為從資料庫或設定檔讀取)
		private static List<MailTemplate> MailTemplates = new()
		{
			new MailTemplate {
				Id = "pending",
				Name = "待處理通知",
				Content = @"
					<p>親愛的客戶您好：</p>
					<p>我們已順利收到您的工單申請，並將盡快為您處理。</p>
					<p>您的問題對我們非常重要，專員會於收到工單後儘速聯繫您，請耐心等候。</p>
					<p>如需補充說明，歡迎直接回覆本信，或利用客服聊天室與我們聯絡。</p>
					<p style='margin-top:1.5em'>謝謝您的支持與信任！<br>貓爪足跡 客服團隊敬上</p>"
				},
			new MailTemplate {
				Id = "processing",
				Name = "處理中通知",
				Content = @"
					<p>親愛的客戶您好：</p>
					<p>您的工單正在由專員處理中，感謝您的耐心等候。</p>
					<p>我們將會在處理有進度時，主動通知您，或有需要協助時也會主動聯繫。</p>
					<p>如果有任何新問題，也歡迎隨時回覆本信或與客服聊天室聯絡。</p>
					<p style='margin-top:1.5em'>貓爪足跡 客服團隊敬上</p>"
				},
			new MailTemplate {
				Id = "finished",
				Name = "已完成通知",
				Content = @"
					<p>親愛的客戶您好：</p>
					<p>您的工單已完成處理，感謝您的耐心等候！</p>
					<p>若您對本次服務有任何建議或回饋，歡迎回覆本信或填寫滿意度問卷，您的意見將協助我們持續進步。</p>
					<p>若日後還有其他疑問或需求，也請隨時與我們聯絡。</p>
					<p style='margin-top:1.5em'>祝您順心愉快！<br>貓爪足跡 客服團隊敬上</p>"
				}
		};

		/// <summary>
		/// 郵件撰寫頁面 (GET)
		/// </summary>
		/// <param name="to">預設收件人</param>
		[HttpGet]
		public IActionResult Compose(string? to)
		{
			var vm = new MailComposeVm
			{
				To = to ?? "",
				Subject = "客服通知信",
				Body = MailTemplates.First().Content, // 預設為第一個範本
				SelectedTemplate = MailTemplates.First().Id,
				Templates = MailTemplates
			};
			return View(vm);
		}

		/// <summary>
		/// 取得指定範本內容 (AJAX)
		/// </summary>
		/// <param name="templateId">範本ID</param>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult GetTemplateContent(string templateId)
		{
			var template = MailTemplates.FirstOrDefault(t => t.Id == templateId);
			if (template == null) return Json(new { ok = false });
			return Json(new { ok = true, content = template.Content });
		}

		/// <summary>
		/// 寄送郵件 (POST)
		/// </summary>
		/// <param name="vm">郵件內容 ViewModel</param>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Send(MailComposeVm vm)
		{
			if (!ModelState.IsValid)
			{
				vm.Templates = MailTemplates;
				return View("Compose", vm); // 驗證失敗回原頁
			}
			try
			{
				await _sender.SendAsync(vm.To, vm.Subject, vm.Body); // 執行寄信
				ViewBag.MailOk = "郵件已送出。";
				// 清空欄位（或依需求保留）
				vm.Body = "";
				vm.To = "";
				vm.Subject = "";
				vm.SelectedTemplate = MailTemplates.First().Id;
			}
			catch (Exception ex)
			{
				ViewBag.MailOk = $"發送失敗: {ex.Message}";
			}
			vm.Templates = MailTemplates;
			return View("Compose", vm);
		}
	}

	/// <summary>
	/// 郵件撰寫 ViewModel
	/// </summary>
	public class MailComposeVm
	{
		public string To { get; set; } = ""; // 收件人
		public string Subject { get; set; } = ""; // 主旨
		public string Body { get; set; } = ""; // 內容
		public string SelectedTemplate { get; set; } = ""; // 選擇的範本
		public List<MailTemplate> Templates { get; set; } = new(); // 所有範本
	}

	/// <summary>
	/// 郵件範本類別
	/// </summary>
	public class MailTemplate
	{
		public string Id { get; set; } = ""; // 範本ID
		public string Name { get; set; } = ""; // 範本名稱
		public string Content { get; set; } = ""; // 範本內容
	}
}