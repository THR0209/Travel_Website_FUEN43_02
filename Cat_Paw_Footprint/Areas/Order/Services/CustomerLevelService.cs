using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;

namespace Cat_Paw_Footprint.Areas.Order.Services
{
	public interface ICustomerLevelService
	{
		Task<int> RecalculateAndUpdateAsync(int customerId);
	}

	public class CustomerLevelService : ICustomerLevelService
	{
		private readonly webtravel2Context _db;
		public CustomerLevelService(webtravel2Context db) => _db = db;

		public async Task<int> RecalculateAndUpdateAsync(int customerId)
		{
			// 1) 撈出該客戶「已付款」訂單總額
			// 假設已付款狀態文字是「已付款」、或你有 OrderStatusID（請自行換成你的常數）
			var total = await _db.CustomerOrders
				.AsNoTracking()
				.Include(o => o.OrderStatus)
				.Where(o => o.CustomerID == customerId && o.OrderStatus!.StatusDesc == "已付款")
				.SumAsync(o => (decimal?)(o.TotalAmount ?? 0) ?? 0m);

			// 2) 套等級門檻
			int level = 0; // 鐵
			if (total >= 30000m) level = 3; // 金
			else if (total >= 15000m) level = 2; // 銀
			else if (total > 0m) level = 1; // 銅（成為會員且消費任意金額）

			// 3) 寫回 Customers.Level
			var c = await _db.Customers.FirstOrDefaultAsync(x => x.CustomerID == customerId);
			if (c != null && c.Level != level)
			{
				c.Level = level;
				await _db.SaveChangesAsync();
			}
			return level;
		}
	}
}
