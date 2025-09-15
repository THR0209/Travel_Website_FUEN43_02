using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// FAQ 與 FAQ 分類資料存取層，與 DB 直接互動
	/// </summary>
	public class FAQRepository : IFAQRepository
	{
		private readonly webtravel2Context _context;
		/// <summary>
		/// 透過 DI 注入 DbContext
		/// </summary>
		public FAQRepository(webtravel2Context context) => _context = context;

		// ===== FAQ CRUD =====

		/// <summary>
		/// 透過 DI 注入 DbContext
		/// </summary>
		public async Task<List<FAQs>> GetAllFAQsAsync()
			=> await _context.FAQs.Include(f => f.Category).ToListAsync();

		/// <summary>
		/// 依據 FAQ ID 取得單筆 FAQ，包含分類
		/// </summary>
		public async Task<FAQs?> GetFAQByIdAsync(int id)
			=> await _context.FAQs.Include(f => f.Category).FirstOrDefaultAsync(f => f.FAQID == id);

		/// <summary>
		/// 新增一筆 FAQ
		/// </summary>
		public async Task AddFAQAsync(FAQs faq)
		{
			_context.FAQs.Add(faq);
			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// 更新一筆 FAQ
		/// </summary>
		public async Task UpdateFAQAsync(FAQs faq)
		{
			var entity = await _context.FAQs.FindAsync(faq.FAQID);
			if (entity == null) return;

			entity.Question = faq.Question;
			entity.Answer = faq.Answer;
			entity.CategoryID = faq.CategoryID;
			entity.IsActive = faq.IsActive;
			entity.UpdateTime = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// 刪除 FAQ（依 ID）
		/// </summary>
		public async Task DeleteFAQAsync(int id)
		{
			var faq = await _context.FAQs.FindAsync(id);
			if (faq == null) return;
			_context.FAQs.Remove(faq);
			await _context.SaveChangesAsync();
		}

		// ===== Category CRUD =====

		/// <summary>
		/// 取得所有 FAQ 分類（不追蹤實體）
		/// </summary>
		public async Task<List<FAQCategorys>> GetCategoriesAsync()
			=> await _context.FAQCategorys.AsNoTracking().ToListAsync();

		/// <summary>
		/// 依據分類 ID 取得單筆分類
		/// </summary>

		public async Task<FAQCategorys?> GetCategoryByIdAsync(int id)
			=> await _context.FAQCategorys.FindAsync(id);

		/// <summary>
		/// 新增 FAQ 分類
		/// </summary>
		public async Task AddCategoryAsync(FAQCategorys category)
		{
			_context.FAQCategorys.Add(category);
			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// 更新 FAQ 分類
		/// </summary>
		public async Task UpdateCategoryAsync(FAQCategorys category)
		{
			var existing = await _context.FAQCategorys.FindAsync(category.CategoryID);
			if (existing == null) throw new KeyNotFoundException("Category not found");
			existing.CategoryName = category.CategoryName;
			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// 刪除 FAQ 分類（先檢查該分類下是否仍有 FAQ，若有則不允許刪除）
		/// </summary>
		public async Task DeleteCategoryAsync(int id)
		{
			// 先檢查該分類下是否還有 FAQ，若有則拋出例外
			var hasFaqs = await _context.FAQs.AnyAsync(f => f.CategoryID == id);
			if (hasFaqs) throw new InvalidOperationException("該分類下仍有 FAQ，不能刪除");

			var entity = await _context.FAQCategorys.FindAsync(id);
			if (entity != null)
			{
				_context.FAQCategorys.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}
	}
}