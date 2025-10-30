using System;
using System.Linq;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Services
{
    public class MemberLevelService
    {
        private readonly webtravel2Context _context;

        public MemberLevelService(webtravel2Context context)
        {
            _context = context;
        }

        /// <summary>
        /// 檢查會員是否該升級，若升級則自動更新等級並發放對應優惠券
        /// </summary>
        public void CheckAndUpgradeMember(int customerId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerID == customerId);
            if (customer == null) return;

            // 計算已付款訂單總額
            decimal totalPaid = _context.CustomerOrders
                .Where(o => o.CustomerID == customerId && o.OrderStatusID == 1) // 1 = 已付款
                .Select(o => o.TotalAmount ?? 0)
                .DefaultIfEmpty(0)
                .Sum();

            int currentLevel = customer.Level;
            int newLevel = currentLevel;

            // 判斷升級條件
            if (totalPaid >= 45000) newLevel = 3;   // 金
            else if (totalPaid >= 15000) newLevel = 2; // 銀
            else if (totalPaid >= 200) newLevel = 1;   // 銅
            else newLevel = 0;                         // 鐵

            // 若等級有變化
            if (newLevel != currentLevel)
            {
                customer.Level = newLevel;
                _context.SaveChanges();

                // 發放對應等級優惠券
                string targetType = newLevel switch
                {
                    1 => "Level_Bronze",
                    2 => "Level_Silver",
                    3 => "Level_Gold",
                    _ => "Level_Iron"
                };

                GrantCouponsForType(customerId, targetType);
            }
        }

        /// <summary>
        /// 發放指定類型的優惠券（包含新註冊與等級升級）
        /// </summary>
        public void GrantCouponsForType(int customerId, string targetType)
        {
            var now = DateTime.Now;

            var coupons = _context.Coupons
                .Where(c => c.IsActive)
                .Where(c => c.TargetType == targetType)
                .Where(c => c.StartDate <= now && c.EndDate >= now)
                .ToList();

            foreach (var cpn in coupons)
            {
                bool alreadyHas = _context.CustomerCouponsRecords
                    .Any(r => r.CustomerID == customerId && r.CouponID == cpn.CouponID);

                if (!alreadyHas)
                {
                    _context.CustomerCouponsRecords.Add(new CustomerCouponsRecords
                    {
                        CustomerID = customerId,
                        CouponID = cpn.CouponID,
                        IsUsed = false
                    });
                }
            }

            _context.SaveChanges();
        }
    }
}
