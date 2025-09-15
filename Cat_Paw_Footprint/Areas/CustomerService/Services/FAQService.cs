using Cat_Paw_Footprint.Areas.CustomerService.Repositories;
using Cat_Paw_Footprint.Models;
using static Cat_Paw_Footprint.Areas.CustomerService.ViewModel.FAQServiceDashboardViewModel;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// FAQ 與 FAQ 分類 服務層，集中商業邏輯
	/// </summary>
	public class FAQService : IFAQService
	{
		private readonly IFAQRepository _repository;
		/// <summary>
		/// 透過 DI 注入 FAQ Repository
		/// </summary>
		public FAQService(IFAQRepository repository) => _repository = repository;

		// ===== FAQ CRUD =====
		/// <summary>
		/// 透過 DI 注入 FAQ Repository
		/// </summary>
		public async Task<List<FAQViewModel>> GetAllFAQsAsync()
		{
			var faqs = await _repository.GetAllFAQsAsync();
			// 將 Entity 轉成 ViewModel 回傳
			return faqs.Select(f => new FAQViewModel
			{
				FAQID = f.FAQID,
				Question = f.Question,
				Answer = f.Answer,
				CategoryID = f.CategoryID,
				CategoryName = f.Category?.CategoryName,
				IsActive = f.IsActive,
				CreateTime = f.CreateTime,
				UpdateTime = f.UpdateTime
			}).ToList();
		}

		/// <summary>
		/// 依據 FAQ ID 取得單一 FAQ，轉成 ViewModel
		/// </summary>
		public async Task<FAQViewModel?> GetFAQByIdAsync(int id)
		{
			var f = await _repository.GetFAQByIdAsync(id);
			if (f == null) return null;
			return new FAQViewModel
			{
				FAQID = f.FAQID,
				Question = f.Question,
				Answer = f.Answer,
				CategoryID = f.CategoryID,
				CategoryName = f.Category?.CategoryName,
				IsActive = f.IsActive,
				CreateTime = f.CreateTime,
				UpdateTime = f.UpdateTime
			};
		}

		/// <summary>
		/// 新增 FAQ，將 ViewModel 轉成 Entity 並存進資料庫
		/// </summary>
		public async Task AddFAQAsync(FAQViewModel faqVm)
		{
			var entity = new FAQs
			{
				Question = faqVm.Question,
				Answer = faqVm.Answer,
				CategoryID = faqVm.CategoryID ?? 0, // 若沒有選擇分類則預設 0
				IsActive = faqVm.IsActive,
				CreateTime = DateTime.Now,
				UpdateTime = DateTime.Now
			};
			await _repository.AddFAQAsync(entity); // 存入資料庫
		}

		/// <summary>
		/// 更新 FAQ，先取出原本的資料再進行修改
		/// </summary>
		public async Task UpdateFAQAsync(FAQViewModel faqVm)
		{
			var existing = await _repository.GetFAQByIdAsync(faqVm.FAQID);
			if (existing == null) throw new KeyNotFoundException("FAQ not found");

			existing.Question = faqVm.Question;
			existing.Answer = faqVm.Answer;
			existing.CategoryID = faqVm.CategoryID ?? existing.CategoryID;
			existing.IsActive = faqVm.IsActive;
			existing.UpdateTime = DateTime.Now;

			await _repository.UpdateFAQAsync(existing); // 更新資料庫
		}
		/// <summary>
		/// 刪除 FAQ
		/// </summary>
		public async Task DeleteFAQAsync(int id) => await _repository.DeleteFAQAsync(id);

		// ===== Category CRUD =====

		/// <summary>
		/// 取得所有分類，並轉成 ViewModel
		/// </summary>
		public async Task<List<FAQCategoryViewModel>> GetCategoriesAsync()
		{
			var cats = await _repository.GetCategoriesAsync();
			return cats.Select(c => new FAQCategoryViewModel
			{
				CategoryID = c.CategoryID,
				CategoryName = c.CategoryName
			}).ToList();
		}

		/// <summary>
		/// 新增分類，將 ViewModel 轉成 Entity 並存進資料庫
		/// </summary>
		public async Task AddCategoryAsync(FAQCategoryViewModel catVm)
		{
			var entity = new FAQCategorys { CategoryName = catVm.CategoryName };
			await _repository.AddCategoryAsync(entity);
		}

		/// <summary>
		/// 更新分類，先取出原本的資料再進行修改
		/// </summary>
		public async Task UpdateCategoryAsync(FAQCategoryViewModel catVm)
		{
			var existing = await _repository.GetCategoryByIdAsync(catVm.CategoryID ?? 0);
			if (existing == null) throw new KeyNotFoundException("Category not found");
			existing.CategoryName = catVm.CategoryName;
			await _repository.UpdateCategoryAsync(existing);
		}

		/// <summary>
		/// 刪除分類
		/// </summary>
		public async Task DeleteCategoryAsync(int id) => await _repository.DeleteCategoryAsync(id);
	}
}