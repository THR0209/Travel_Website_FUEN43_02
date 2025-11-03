using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// 定義「聊天附件上傳服務」的介面。
	/// 提供統一的檔案儲存與安全檢查邏輯。
	/// </summary>
	public interface IChatAttachmentService
	{
		/// <summary>
		/// 儲存上傳的附件檔案，並回傳可供前端顯示的相對路徑。回傳外部網址（ImgBB）。
		/// </summary>
		/// <param name="file">使用者上傳的檔案</param>
		/// <returns>檔案相對路徑（例如 /uploads/chat/xxxx.png）</returns>
		Task<string> SaveFileAsync(IFormFile file);
	}
}
