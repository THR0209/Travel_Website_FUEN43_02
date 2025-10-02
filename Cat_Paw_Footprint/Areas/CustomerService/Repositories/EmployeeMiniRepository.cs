using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 員工資料存取簡易實作，取得員工ID與姓名字典
	/// </summary>
	public class EmployeeMiniRepository : IEmployeeMiniRepository
	{
		private readonly webtravel2Context _context;

		/// <summary>
		/// 透過 DI 注入 DbContext
		/// </summary>
		public EmployeeMiniRepository(webtravel2Context context) => _context = context;

		/// <summary>
		/// 取得所有員工ID與員工姓名對應字典
		/// </summary>
		public async Task<IDictionary<int, string>> GetEmployeeNamesAsync()
		{
			return await _context.Employees
				.Include(e => e.EmployeeProfile) // 包含員工個人資料
				.Where(e => e.EmployeeProfile != null) // 排除未建立個人檔案者
				.ToDictionaryAsync(
					e => e.EmployeeID,
					e => e.EmployeeProfile.EmployeeName ?? "(未知員工)" // 如果姓名為 null 則顯示 (未知員工)
				);
		}
	}
}