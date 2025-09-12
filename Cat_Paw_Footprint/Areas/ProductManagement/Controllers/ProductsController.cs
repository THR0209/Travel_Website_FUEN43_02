using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Cat_Paw_Footprint.Areas.ProductManagement.Controllers
{
    [Area("ProductManagement")]
    public class ProductsController : Controller
    {
        private readonly webtravel2Context _context;

        public ProductsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: ProductManagement/Products
        public async Task<IActionResult> Index()
        {
            var webtravel2Context = _context.Products.Include(p => p.Region);
            return View(await webtravel2Context.ToListAsync());
        }

        [HttpPost]
        //[Route("Products/Index/Json")]
        [Route("ProductManagement/Products/Index/Json")]
        public async Task<IActionResult> IndexJson()
        {
            var data = await _context.Products
            .Include(p => p.Region)
            .Include(p => p.ProductAnalyses) // 更新912
            .Select(p => new {
                p.ProductCode,
                p.ProductImage,
                p.ProductName,
                p.ProductPrice,
                StartDate = p.StartDate.HasValue ? p.StartDate.Value.ToString("yyyy-MM-dd") : "",
                EndDate = p.EndDate.HasValue ? p.EndDate.Value.ToString("yyyy-MM-dd") : "",
                p.MaxPeople,
                p.RegionID,
				RegionName = p.Region == null ? null : p.Region.RegionName,
                Analyses = p.ProductAnalyses.Select(a => new {
					releaseDate =  a.ReleaseDate.HasValue ? a.ReleaseDate.Value.ToString("yyyy-MM-dd") : "",
					removalDate = a.RemovalDate.HasValue ? a.RemovalDate.Value.ToString("yyyy-MM-dd") : ""
				})
            })
            .ToListAsync();
            //var webtravel2Context = _context.Products;//.Include(p => p.Region);
            return Json(data);
        }

        // GET: ProductManagement/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Region)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: ProductManagement/Products/Create
        public IActionResult Create()
        {
            var model = new Products();

            /* 取得流水號 */
            //model.ProductCode = GetCodeWithDate("Products", "PRO");

            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName");
            return View();
        }

		public IActionResult Create_Page2()
		{
			var model = new Products();

			/* 取得流水號 */
			//model.ProductCode = GetCodeWithDate("Products", "PRO");

			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName");
			return View();
		}

		public string? GetCodeWithDate(string tablename, string prefix)
        {

            string ConStr = "Server=tcp:web-travel.database.windows.net,1433;Database=web-travel2;User Id=webTravel-ap@web-travel;Password=P9L9M6+n6m8B]hw;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";

            using (SqlConnection conn = new SqlConnection(ConStr))
            {
                using (SqlCommand cmd = new SqlCommand("GetNewSerialNumberWithDate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SourceTable", tablename);
					cmd.Parameters.AddWithValue("@Prefix", prefix);

                    /* NewSerial 是 預存程序中的 output 參數名稱 */
					var outputParam = new SqlParameter("@NewSerial", SqlDbType.VarChar, 20)
					{
                        /* 設定參數是要 output，預設是 input */
						Direction = ParameterDirection.Output
					};
					cmd.Parameters.Add(outputParam);

					conn.Open();
					cmd.ExecuteNonQuery();

					string? newCode = outputParam.Value.ToString();

                    return newCode;
				}
            }
        }

		public string? GetCodeWithNoDate(string tablename, string prefix)
		{
			string ConStr = "Server=tcp:web-travel.database.windows.net,1433;Database=web-travel2;User Id=webTravel-ap@web-travel;Password=P9L9M6+n6m8B]hw;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";

			using (SqlConnection conn = new SqlConnection(ConStr))
			{
				using (SqlCommand cmd = new SqlCommand("GetNewSerialNumberWithNoDate", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@SourceTable", tablename);
					cmd.Parameters.AddWithValue("@Prefix", prefix);

					/* NewSerial 是 預存程序中的 output 參數名稱 */
					var outputParam = new SqlParameter("@NewSerial", SqlDbType.VarChar, 20)
					{
						/* 設定參數是要 output，預設是 input */
						Direction = ParameterDirection.Output
					};
					cmd.Parameters.Add(outputParam);

					conn.Open();
					cmd.ExecuteNonQuery();

					string? newCode = outputParam.Value.ToString();

					return newCode;
				}
			}
		}

		// POST: ProductManagement/Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,RegionID,ProductDesc,ProductPrice,ProductNote,StartDate,EndDate,MaxPeople,ProductImage,ProductCode")] Products products)
        {
			if (ModelState.IsValid)
            {
                /* 取得流水號 */
                products.ProductCode = GetCodeWithDate("Products", "PRO");

                /* 將圖片轉成二進位並存進資料庫 */
				if (Request.Form.Files["ProductImage"] != null)
				{
					using (BinaryReader br = new BinaryReader(Request.Form.Files["ProductImage"].OpenReadStream()))
					{
						products.ProductImage = br.ReadBytes((int)Request.Form.Files["ProductImage"].Length);
					}
				}
				_context.Add(products);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", products.RegionID);
            return View(products);
        }

        // GET: ProductManagement/Products/Edit/5
        [Route("ProductManagement/Products/Edit/{productCode?}")]
        public async Task<IActionResult> Edit(string? productCode)
        {
			if (productCode == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FirstOrDefaultAsync(p => p.ProductCode == productCode);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", products.RegionID);
            return View(products);
        }

        // POST: ProductManagement/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
		[Route("ProductManagement/Products/Edit/{productCode?}")]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string productCode, [Bind("ProductID,ProductName,RegionID,ProductDesc,ProductPrice,ProductNote,StartDate,EndDate,MaxPeople,ProductImage,ProductCode")] Products products)
        {
			if (productCode != products.ProductCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", products.RegionID);
            return View(products);
        }

        // GET: ProductManagement/Products/Delete/5
        [HttpGet]
		[Route("ProductManagement/Products/Delete/{productCode?}")]
		public async Task<IActionResult> Delete(string? productCode)
        {
			if (productCode == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Region)
                .FirstOrDefaultAsync(m => m.ProductCode == productCode);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }



		// POST: ProductManagement/Products/Delete/5
		[HttpPost, ActionName("Delete")]
		[Route("ProductManagement/Products/Delete/{productCode?}")]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string productCode)
        {
			var products = await _context.Products.FirstOrDefaultAsync(p => p.ProductCode == productCode);
            if (products != null)
            {
                _context.Products.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(string id)
        {
            return _context.Products.Any(e => e.ProductCode == id);
        }
    }
}
