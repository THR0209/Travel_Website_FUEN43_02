using static Cat_Paw_Footprint.Areas.CustomerService.ViewModel.FAQServiceDashboardViewModel;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	// FAQ 服務介面，定義 FAQ 與 FAQ 分類的 CRUD 操作
	public interface IFAQService
	{
		// FAQ 相關 CRUD 操作

		/// <summary>
		/// 取得所有 FAQ 清單 (非同步)
		/// </summary>
		Task<List<FAQViewModel>> GetAllFAQsAsync();
		/// <summary>
		/// 依據 FAQ ID 取得單一 FAQ (非同步)
		/// </summary>
		Task<FAQViewModel?> GetFAQByIdAsync(int id);
		/// <summary>
		/// 依據 FAQ ID 取得單一 FAQ (非同步)
		/// </summary>
		Task AddFAQAsync(FAQViewModel faqVm);
		/// <summary>
		/// 更新 FAQ (非同步)
		/// </summary>
		Task UpdateFAQAsync(FAQViewModel faqVm);
		/// <summary>
		/// 刪除 FAQ (非同步)
		/// </summary>
		Task DeleteFAQAsync(int id);


		// FAQ 分類相關 CRUD 操作

		/// <summary>
		/// 取得所有 FAQ 分類 (非同步)
		/// </summary>
		Task<List<FAQCategoryViewModel>> GetCategoriesAsync();
		/// <summary>
		/// 新增 FAQ 分類 (非同步)
		/// </summary>
		Task AddCategoryAsync(FAQCategoryViewModel category);
		/// <summary>
		/// 更新 FAQ 分類 (非同步)
		/// </summary>
		Task UpdateCategoryAsync(FAQCategoryViewModel category);
		/// <summary>
		/// 刪除 FAQ 分類 (非同步)
		/// </summary>
		Task DeleteCategoryAsync(int id);
	}
}