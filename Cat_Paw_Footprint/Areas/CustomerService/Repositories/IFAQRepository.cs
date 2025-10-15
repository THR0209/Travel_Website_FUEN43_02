using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// FAQ 與 FAQ 分類資料存取介面，定義與資料庫互動的方法
	/// </summary>
	public interface IFAQRepository
	{
		// FAQ CRUD

		/// <summary>
		/// 取得所有 FAQ 資料
		/// </summary>
		Task<List<FAQs>> GetAllFAQsAsync();

		/// <summary>
		/// 依據 FAQ ID 取得單筆 FAQ
		/// </summary>
		Task<FAQs?> GetFAQByIdAsync(int id);

		/// <summary>
		/// 新增一筆 FAQ
		/// </summary>
		Task AddFAQAsync(FAQs faq);

		/// <summary>
		/// 更新一筆 FAQ
		/// </summary>
		Task UpdateFAQAsync(FAQs faq);

		/// <summary>
		/// 刪除 FAQ（依 ID）
		/// </summary>
		Task DeleteFAQAsync(int id);

		// Category CRUD

		/// <summary>
		/// 取得所有 FAQ 分類資料
		/// </summary>
		Task<List<FAQCategorys>> GetCategoriesAsync();

		/// <summary>
		/// 依據分類 ID 取得單筆分類
		/// </summary>
		Task<FAQCategorys?> GetCategoryByIdAsync(int id);

		/// <summary>
		/// 新增 FAQ 分類
		/// </summary>
		Task AddCategoryAsync(FAQCategorys category);

		/// <summary>
		/// 更新 FAQ 分類
		/// </summary>
		Task UpdateCategoryAsync(FAQCategorys category);

		/// <summary>
		/// 刪除 FAQ 分類
		/// </summary>
		Task DeleteCategoryAsync(int id);
	}
}