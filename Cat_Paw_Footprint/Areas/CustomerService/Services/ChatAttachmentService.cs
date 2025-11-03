using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cat_Paw_Footprint.Areas.Helper;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// 聊天附件上傳服務實作（僅支援圖片，上傳到 ImgBB）。
	/// </summary>
	public class ChatAttachmentService : IChatAttachmentService
	{
		public async Task<string> SaveFileAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
				throw new InvalidOperationException("未選擇任何檔案。");

			// ✅ 檔案大小上限 25MB
			const long maxFileSize = 25 * 1024 * 1024;
			if (file.Length > maxFileSize)
				throw new InvalidOperationException("檔案大小不可超過 25MB。");

			// ✅ 僅允許圖片格式
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
			var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

			if (!allowedExtensions.Contains(ext))
				throw new InvalidOperationException("僅允許上傳圖片格式（JPG、PNG、GIF、WEBP）。");

			try
			{
				// ✅ 上傳至 ImgBB
				var url = await ImgBBHelper.UploadSingleImageAsync(file);
				if (string.IsNullOrWhiteSpace(url))
					throw new InvalidOperationException("ImgBB 回傳空網址，上傳可能失敗。");

				return url;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"圖片上傳至 ImgBB 失敗：{ex.Message}");
			}
		}
	}
}
