using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Data;

namespace Cat_Paw_Footprint.Areas.CustomerArea.Order.Controllers
{
    [Area("CustomerArea")]
    [Route("CustomerArea/Order/[controller]")]
    public class PaymentController : Controller
    {
        private readonly webtravel2Context _db;
        public PaymentController(webtravel2Context db) => _db = db;

        // 模擬付款頁（?orderId=）
        [HttpGet("mock")]
        public async Task<IActionResult> Mock([FromQuery] int orderId)
        {
            var o = await _db.CustomerOrders
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.OrderStatus)
                .FirstOrDefaultAsync(x => x.OrderID == orderId);

            if (o == null) return NotFound();
            ViewData["OrderId"] = o.OrderID;
            ViewData["OrderCode"] = $"ORD-{(o.CreateTime?.ToString("yyyyMMdd-HHmmss") ?? "NA")}-{o.OrderID}";
            ViewData["ProductName"] = o.Product?.ProductName ?? $"商品 {o.ProductID}";
            ViewData["Amount"] = o.TotalAmount ?? 0;
            ViewData["Status"] = o.OrderStatus?.StatusDesc ?? "";

            return View("MockPay");
        }

        // 模擬付款送出：把狀態改「已付款(1)」
        [HttpPost("mock/submit")]
        public async Task<IActionResult> MockSubmit([FromForm] int orderId, [FromForm] string cardNo)
        {
            var o = await _db.CustomerOrders.FirstOrDefaultAsync(x => x.OrderID == orderId);
            if (o == null) return NotFound();

            // 當作卡號合法
            o.OrderStatusID = 1; // 已付款
            o.UpdateTime = DateTime.Now;
            await _db.SaveChangesAsync();

            TempData["PayOk"] = "付款成功";
            return RedirectToAction("Index", "Orders", new { area = "CustomerArea", orderId });
        }
    }
}
