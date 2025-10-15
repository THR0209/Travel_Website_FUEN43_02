using Cat_Paw_Footprint.Areas.Order.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Cat_Paw_Footprint.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("Order/[controller]")]
    [Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaOrder")]
	public class CustomerOrdersController : Controller
    {
		private readonly webtravel2Context _context;
		private readonly IEmailSender _sender;

		// ✅ 同時注入 DbContext 與 EmailSender
		public CustomerOrdersController(webtravel2Context context, IEmailSender sender)
		{
			_context = context;
			_sender = sender;
		}

		// GET: Order/CustomerOrders
		public async Task<IActionResult> Index()
        {
            var webtravel2Context = _context.CustomerOrders.Include(c => c.CustomerProfile).Include(c => c.CustomerProfile).Include(c => c.OrderStatus).Include(c => c.Product);
            return View(await webtravel2Context.ToListAsync());
        }

        private bool CustomerOrdersExists(int id)
        {
            return _context.CustomerOrders.Any(e => e.OrderID == id);
        }
        [HttpPost("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,ProductID,OrderID,OrderStatusID,TotalAmount,CreateTime,UpdateTime")] CustomerOrders customerOrders)
        {
            if (id != customerOrders.OrderID) return NotFound();
            if (!ModelState.IsValid) return View(customerOrders);

            _context.Update(customerOrders);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet("by-product/{productId:int}")]
        public async Task<IActionResult> ByProduct(int productId)
        {
            var q = _context.CustomerOrders
                            .AsNoTracking()
                            .Include(o => o.CustomerProfile)
                            .Include(o => o.OrderStatus)
                            .Include(o => o.Product)
                            .Where(o => o.ProductID == productId)
                            .OrderByDescending(o => o.CreateTime);

            var orders = await q.Select(o => new {
                id = o.OrderID,
                orderCode = "ORD-" + o.CreateTime.Value.ToString("yyyyMMdd" + "-" + $"{o.OrderID}"),   
                totalAmount = o.TotalAmount,
                createTime = o.CreateTime,
                updateTime = o.UpdateTime,
                customer = o.CustomerProfile != null? (o.CustomerProfile.CustomerName ?? o.CustomerProfile.CustomerID.ToString()) : "",
                status = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
				customerEmail = o.CustomerProfile != null ? o.CustomerProfile.Email : "",
				product = o.Product != null ? o.Product.ProductID : 0
            }).ToListAsync();

            return Ok(new
            {
                orders,
                kpi = new
                {
                    total = orders.Count,
                    totalAmount = orders.Sum(x => (decimal?)x.totalAmount ?? 0m)
                }
            });
        }

        [HttpGet("get/{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var o = await _context.CustomerOrders
                .AsNoTracking()
                .Include(x => x.CustomerProfile)
                .Include(x => x.OrderStatus)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.OrderID == id);

            if (o == null) return NotFound();

            return Ok(new
            {
                id = o.OrderID,
                orderCode = "ORD-" + o.CreateTime.Value.ToString("yyyyMMdd" + "-" + $"{o.OrderID}"),
                status = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
                customerId = o.CustomerID,
                customerName = o.CustomerProfile != null ? o.CustomerProfile.CustomerName : "",
                productId = o.ProductID,
                orderStatusId = o.OrderStatusID,
				customerEmail = o.CustomerProfile?.Email,
				totalAmount = o.TotalAmount,
                createTime = o.CreateTime,
                updateTime = o.UpdateTime,
            });
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export(DateTime? start, DateTime? end)
        {

            // 基礎查詢（連同關聯）
            var q = _context.CustomerOrders
                .AsNoTracking()
                .Include(x => x.CustomerProfile)
                .Include(x => x.Product)
                .Include(x => x.OrderStatus)
                .AsQueryable();

            // 時間範圍篩選（使用 CreateTime）
            if (start.HasValue) q = q.Where(o => o.CreateTime >= start);
            if (end.HasValue) q = q.Where(o => o.CreateTime <= end);


            var data = await q
                .OrderBy(o => o.CreateTime)
                .Select(o => new
                {
                    訂單編號 = "ORD-" + (o.CreateTime.HasValue ? o.CreateTime.Value.ToString("yyyyMMdd") : "NA") + "-" + o.OrderID,
                    建立時間 = o.CreateTime,
                    更新時間 = o.UpdateTime,
                    客戶 = o.CustomerProfile != null ? o.CustomerProfile.CustomerName : o.CustomerID.ToString(),
                    客戶Email = o.CustomerProfile != null ? o.CustomerProfile.Email : "",
                    商品 = o.Product != null ? o.Product.ProductName : o.ProductID.ToString(),
                    狀態 = o.OrderStatus != null ? o.OrderStatus.StatusDesc : "",
                    總金額 = o.TotalAmount ?? 0
                })
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Orders");
            // 一行丟整個匿名型別清單會自動生表頭
            ws.Cell(1, 1).InsertTable(data, "Orders", true);
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;

            string fname = $"orders_{(start.HasValue ? start.Value.ToString("yyyyMMdd") : "all")}_{(end.HasValue ? end.Value.ToString("yyyyMMdd") : "all")}.xlsx";
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fname);
        }
        [HttpPost("send-ajax")]
        public async Task<IActionResult> SendAjax([FromForm] string to, [FromForm] string subject, [FromForm] string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                return BadRequest(new { ok = false, error = "收件者信箱為必填" });

            try
            {
                await _sender.SendAsync(to, subject ?? "(無主旨)", body ?? "");
                return Ok(new { ok = true });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"SMTP 發送錯誤: {ex}");
                return BadRequest(new { ok = false, error = ex.Message });
            }
        }
        [HttpGet("Order/CustomerOrders/mail-template/{id:int}")]
        public async Task<IActionResult> MailTemplate(int id)
        {
            var o = await _context.CustomerOrders
                .AsNoTracking()
                .Include(x => x.OrderStatus)
                .Include(x => x.Product)
                .Include(x => x.CustomerProfile) // 有 Email/姓名就一起帶
                .FirstOrDefaultAsync(x => x.OrderID == id);

            if (o == null) return NotFound();

            var (subject, html) = EmailSender.BuildForOrder(o);

            return Ok(new
            {
                to = o.CustomerProfile?.Email ?? "",
                subject,
                body = html
            });
        }
        // 1) 產品聚合（卡片/表格上方的商品列表）
        [HttpGet("api-json/products")]
        public async Task<IActionResult> Products()
        {
            var data = await _context.CustomerOrders
                .Include(o => o.Product)
                .GroupBy(o => o.ProductID)
                .Select(g => new {
                    productId = g.Key,
                    productName = g.Max(x => x.Product!.ProductName),
                    count = g.Count(),
                    imageUrl = _context.Products
                                   .Where(p => p.ProductID == g.Key)
                                   .Select(p => p.ProductImage != null
                                       ? "data:image/png;base64," + Convert.ToBase64String(p.ProductImage)
                                       : @Url.Content("~/images/NoImage.png"))
                                   .FirstOrDefault()
                })
                .OrderBy(x => x.productId)
                .ToListAsync();

            return Ok(data);
        }

        // 2) 指定商品的訂單列表
        [HttpGet("api-json/by-product/{productId:int}")]
        public async Task<IActionResult> ByProductJson(int productId)
        {
            var orders = await _context.CustomerOrders
                .AsNoTracking()
                .Include(o => o.CustomerProfile)
                .Include(o => o.Product)
                .Include(o => o.OrderStatus)
                .Where(o => o.ProductID == productId)
                .OrderByDescending(o => o.CreateTime)
                .Select(o => new {
                    id = o.OrderID,
                    orderCode = "ORD-" + o.CreateTime.Value.ToString("yyyyMMdd" + "-" + $"{o.OrderID}"),
                    totalAmount = o.TotalAmount ?? 0,
                    createTime = o.CreateTime,
                    updateTime = o.UpdateTime,
                    customerName = o.CustomerProfile!.CustomerName,
                    customerEmail = o.CustomerProfile!.Email,
                    productName = o.Product!.ProductName,
                    status = o.OrderStatus!.StatusDesc
                })
                .ToListAsync();

            var kpi = new { total = orders.Count, totalAmount = orders.Sum(x => x.totalAmount) };
            return Ok(new { orders, kpi });
        }

        // 3) 編輯（JSON 版；避免 Razor 的 AntiForgery）
        public record EditOrderDto(int CustomerID, int ProductID, int OrderStatusID, int TotalAmount, DateTime? CreateTime, DateTime? UpdateTime);

        [HttpPut("api-json/{id:int}")]
        public async Task<IActionResult> EditJson(int id, [FromBody] EditOrderDto dto)
        {
            var o = await _context.CustomerOrders.FirstOrDefaultAsync(x => x.OrderID == id);
            if (o == null) return NotFound();

            o.CustomerID = dto.CustomerID;
            o.ProductID = dto.ProductID;
            o.OrderStatusID = dto.OrderStatusID;
            o.TotalAmount = dto.TotalAmount;
            o.CreateTime = dto.CreateTime;
            o.UpdateTime = dto.UpdateTime ?? DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("api-json/orders")]        public async Task<IActionResult> AllOrders()
        {
            var list = await _context.CustomerOrders
                .AsNoTracking()
                .Include(x => x.CustomerProfile)
                .Include(x => x.OrderStatus)
                .Include(x => x.Product)
                .OrderByDescending(x => x.CreateTime)
                .Select(o => new {
                    id = o.OrderID,
                    orderCode = "ORD-" + o.CreateTime.Value.ToString("yyyyMMdd" + "-" + $"{o.OrderID}"),
                    totalAmount = o.TotalAmount,
                    createTime = o.CreateTime,
                    updateTime = o.UpdateTime,
                    customerId = o.CustomerID,
                    customerName = o.CustomerProfile != null ? o.CustomerProfile.CustomerName : "",
                    customerEmail = o.CustomerProfile != null ? o.CustomerProfile.Email : "",
                    productId = o.ProductID,
                    productName = o.Product != null ? o.Product.ProductName : "",
                    statusText = o.OrderStatus != null ? o.OrderStatus.StatusDesc : ""
                })
                .ToListAsync();

            return Json(new { orders = list });   // ★ 一定要包成 { orders: [...] }
        }
        [HttpGet("api-json/statuses")]
        public async Task<IActionResult> Statuses()
        {
            var list = await _context.OrderStatus
                .AsNoTracking()
                .OrderBy(s => s.OrderStatusID)
                .Select(s => new { id = s.OrderStatusID, name = s.StatusDesc })
                .ToListAsync();

            return Ok(list);
        }
        // （選配）寄信模板，讓前端自動帶內容
        [HttpGet("api-json/mail-template/{id:int}")]
        public async Task<IActionResult> MailTemplateJson(int id)
        {
            var o = await _context.CustomerOrders
                .AsNoTracking()
                .Include(x => x.CustomerProfile)
                .Include(x => x.Product)
                .Include(x => x.OrderStatus)
                .FirstOrDefaultAsync(x => x.OrderID == id);
            if (o == null) return NotFound();

            string code = $"ORD-{(o.CreateTime.HasValue ? o.CreateTime.Value.ToString("yyyyMMdd-HHmmss") : "NA")}-{o.OrderID}";
            string name = o.CustomerProfile?.CustomerName ?? $"客戶 {o.CustomerID}";
            string email = o.CustomerProfile?.Email ?? "";
            string amount = (o.TotalAmount ?? 0).ToString("N0");
            string status = o.OrderStatus?.StatusDesc ?? "";
            string createTime = o.CreateTime?.ToString("yyyy/MM/dd HH:mm") ?? "";
            string productName = o.Product?.ProductName ?? $"商品 {o.ProductID}";
            string updateTime = o.UpdateTime?.ToString("yyyy/MM/dd HH:mm") ?? "";

            string subject = $"【訂單通知】{code}";
            string body = status switch
            {
                "已付款" => $@"親愛的 {name} 您好，
感謝您的購買！我們已收到您的款項，以下是您的訂單資訊：
訂單編號：{code}
商品名稱：{productName}
金額：NT$ {amount}
建立時間：{createTime}
狀態：{status}
若您需要發票或任何協助，隨時與我們聯絡。
如有任何問題，歡迎回信與我們聯繫。           貓爪足跡 敬上",
                "未付款" => $@"親愛的 {name} 您好，
您有一筆尚未完成付款的訂單，為確保權益，請盡快完成付款：
訂單編號：{code}
商品名稱：{productName}
金額：NT$ {amount}
建立時間：{createTime}
狀態：{status}
如已付款，請忽略此信或回覆告知，我們將盡快為您確認。
如有任何問題，歡迎回信與我們聯繫。           貓爪足跡 敬上",
                "錯誤" => $@"親愛的 {name} 您好，
很抱歉，您的訂單在處理時發生異常，我們需要與您確認以下資訊：
訂單編號：{code}
商品名稱：{productName}
金額：NT$ {amount}
建立時間：{createTime}
最後更新：{updateTime}
狀態：{status}
麻煩您與客服聯繫，我們將盡速協助處理。
如有任何問題，歡迎回信與我們聯繫。           貓爪足跡 敬上",
                "已取消" => $@"親愛的 {name} 您好，
您所建立的訂單已取消，以下為紀錄資訊：
訂單編號：{ code}
商品名稱：{ productName}
金額：NT$ { amount}
建立時間：{ createTime}
狀態：{ status}
若您並未提出取消，請立即與我們聯繫。
如有任何問題，歡迎回信與我們聯繫。           貓爪足跡 敬上"
            };
            return Ok(new { to = email, subject, body });
        }
        
    }
}
