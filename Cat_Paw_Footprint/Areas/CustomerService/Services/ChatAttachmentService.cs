using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cat_Paw_Footprint.Areas.Helper;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// 聊天附件上傳服務實作。
	/// 改為上傳到 ImgBB 並回傳外部連結。
	/// </summary>
	public class ChatAttachmentService : IChatAttachmentService
	{
		public async Task<string> SaveFileAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
				throw new InvalidOperationException("未選擇任何檔案。");

			const long maxFileSize = 25 * 1024 * 1024; // 25MB
			if (file.Length > maxFileSize)
				throw new InvalidOperationException("檔案大小不可超過 25MB。");

			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf" };
			var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (!allowedExtensions.Contains(ext))
				throw new InvalidOperationException("僅允許上傳圖片或 PDF 檔案。");

			//  若是圖片，上傳 ImgBB
			if (ext != ".pdf")
			{
				try
				{
					var url = await ImgBBHelper.UploadSingleImageAsync(file);
					if (string.IsNullOrWhiteSpace(url))
						throw new InvalidOperationException("ImgBB 回傳空網址，可能上傳失敗。");
					return url;
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"圖片上傳到 ImgBB 失敗：{ex.Message}");
				}
			}

			//  PDF 檔案仍保存於本地
			var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			var chatDir = Path.Combine(uploadRoot, "uploads", "chat");

			if (!Directory.Exists(chatDir))
				Directory.CreateDirectory(chatDir);

			var safeFileName = $"{Guid.NewGuid():N}{ext}";
			var filePath = Path.Combine(chatDir, safeFileName);

			await using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			return $"/uploads/chat/{safeFileName}";
		}
	}
}
