using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// 聊天附件上傳服務實作。
	/// 包含安全檢查、目錄建立、檔案儲存與路徑回傳。
	/// </summary>
	public class ChatAttachmentService : IChatAttachmentService
	{
		private readonly IWebHostEnvironment _env;

		// 建構式注入 IWebHostEnvironment 以取得 wwwroot 實體路徑
		public ChatAttachmentService(IWebHostEnvironment env)
		{
			_env = env;
		}

		/// <summary>
		/// 儲存上傳的檔案並回傳相對路徑。
		/// </summary>
		/// <param name="file">上傳的檔案</param>
		/// <returns>檔案的相對路徑（可前端直接使用）</returns>
		/// <exception cref="InvalidOperationException">當檔案不合法時拋出。</exception>
		public async Task<string> SaveFileAsync(IFormFile file)
		{
			// 1️ 驗證基本條件
			if (file == null || file.Length == 0)
				throw new InvalidOperationException("未選擇任何檔案。");

			// 2️ 檔案大小上限（25MB）
			const long maxFileSize = 25 * 1024 * 1024; // 25MB
			if (file.Length > maxFileSize)
				throw new InvalidOperationException("檔案大小不可超過 25MB。");

			// 3️ 驗證允許的副檔名與 MIME 類型
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf" };
			var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "application/pdf" };

			var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (!allowedExtensions.Contains(ext))
				throw new InvalidOperationException("僅允許上傳圖片或 PDF 檔案。");

			if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
				throw new InvalidOperationException("不支援的檔案格式。");

			// 4️ 建立儲存目錄
			var uploadRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			var chatDir = Path.Combine(uploadRoot, "uploads", "chat");

			if (!Directory.Exists(chatDir))
				Directory.CreateDirectory(chatDir);

			// 5️ 產生唯一檔名
			var safeFileName = $"{Guid.NewGuid():N}{ext}";
			var filePath = Path.Combine(chatDir, safeFileName);

			// 6️ 儲存檔案
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			// 7️ 回傳相對路徑（給前端使用）
			var relativePath = $"/uploads/chat/{safeFileName}";
			return relativePath;
		}
	}
}
