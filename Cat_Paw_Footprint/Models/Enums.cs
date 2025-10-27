namespace Cat_Paw_Footprint.Models
{
    public enum OrderStatusId
    {
        Paid = 1, // 已付款
        Unpaid = 2, // 未付款
        Cancelled = 3, // 已取消（如有）
        Error = 4, // 付款錯誤/失敗（如有）
    }
}
