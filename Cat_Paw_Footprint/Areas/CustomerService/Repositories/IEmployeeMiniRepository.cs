using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	/// <summary>
	/// 員工資料存取簡易介面，定義取得員工ID及姓名的方法
	/// </summary>
	public interface IEmployeeMiniRepository
	{
		/// <summary>
		/// 取得所有員工ID與員工姓名對應字典
		/// </summary>
		Task<IDictionary<int, string>> GetEmployeeNamesAsync();
	}
}