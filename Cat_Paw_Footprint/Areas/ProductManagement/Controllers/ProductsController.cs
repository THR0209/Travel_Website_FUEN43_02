using Cat_Paw_Footprint.Areas.ProductManagement.ViewModel;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.ProductManagement.Controllers
{
    [Area("ProductManagement")]
	[Authorize(AuthenticationSchemes = "EmployeeAuth", Policy = "AreaProductManagement")]
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
            .OrderByDescending(p => p.ProductCode)
            .Select(p => new {
				p.ProductID,
                p.ProductCode,
                p.ProductImage,
                p.ProductName,
                p.ProductPrice,
                StartDate = p.StartDate.HasValue ? p.StartDate.Value.ToString("yyyy-MM-dd") : "",
                EndDate = p.EndDate.HasValue ? p.EndDate.Value.ToString("yyyy-MM-dd") : "",
                p.MaxPeople,
                p.RegionID,
                p.IsActive,
				RegionName = p.Region == null ? null : p.Region.RegionName,
                Analyses = p.ProductAnalyses.Select(a => new {
					releaseDate =  a.ReleaseDate.HasValue ? a.ReleaseDate.Value.ToString("yyyy-MM-dd") : "",
					removalDate = a.RemovalDate.HasValue ? a.RemovalDate.Value.ToString("yyyy-MM-dd") : ""
				})
            })
            .ToListAsync();

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
				.Include(p => p.ProductHotels)
					.ThenInclude(ph => ph.Hotel)
				.Include(p => p.ProductRestaurants)
					.ThenInclude(pr => pr.Restaurant)
				.Include(p => p.ProductsTransportations)
					.ThenInclude(pt => pt.Transport)
				.Include(p => p.ProductLocations)
					.ThenInclude(pl => pl.Location)
				.Include(p => p.ProductAnalyses)
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
			Console.WriteLine("=== [DEBUG] Create POST ===");

			var vm = new ProductCreateViewModel()
            {
                Product = new Products(),
                ProductAnalysis = new ProductAnalysis()
            };


            /* 取得流水號 */
            //model.ProductCode = GetCodeWithDate("Products", "PRO");

            ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName");
            return View(vm);
        }

		// POST: ProductManagement/Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel vm)
        {
			Console.WriteLine("=== [DEBUG] Create POST ===");
			Console.WriteLine("ModelState.IsValid = " + ModelState.IsValid);

			// 抓出所有 ModelState 的錯誤
			if (!ModelState.IsValid)
			{
				// 把錯誤收集起來
				var allErrors = ModelState
					.Where(ms => ms.Value.Errors.Any())
					.Select(ms => new
					{
						Key = ms.Key,
						Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
					});

				// 丟進 ViewBag（讓 Razor 頁面也能看到）
				ViewBag.ModelErrors = allErrors;

				// 確保下拉選單不會因為回傳而消失
				ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", vm.Product.RegionID);

				return View(vm); // 回到表單，錯誤訊息會顯示
			}

			if (ModelState.IsValid)
            {
				/* 取得流水號 */
				//products.ProductCode = GetCodeWithDate("Products", "PRO");

				/* 將圖片轉成二進位並存進資料庫 */
				if (vm.UploadImage != null && vm.UploadImage.Length > 0)
				{
					using var br = new BinaryReader(vm.UploadImage.OpenReadStream());
					vm.Product.ProductImage = br.ReadBytes((int)vm.UploadImage.Length);
				}
				// ===== 1. 存主表Products並產生ProductID =====

				_context.Add(vm.Product);
                await _context.SaveChangesAsync();

				int productId = vm.Product.ProductID;

				// ===== 2. 存多對多關聯 =====

				// (a) 交通方式
				if (vm.SelectedTransportationIds != null && vm.SelectedTransportationIds.Any())
				{
					foreach (var tId in vm.SelectedTransportationIds)
					{
						_context.Products_Transportations.Add(new Products_Transportations
						{
							ProductID = productId,
							TransportID = tId
						});
					}
				}

				// (b) 飯店
				if (vm.SelectedHotelIds != null && vm.SelectedHotelIds.Any())
				{
					foreach (var hId in vm.SelectedHotelIds)
					{
						_context.Products_Hotels.Add(new Products_Hotels
						{
							ProductID = productId,
							HotelID = hId.ID,
							OrderIndex = hId.OrderIndex
						});
					}
				}

				// (c) 餐廳 – 早餐
				if (vm.SelectedRestaurantsBreakfast != null && vm.SelectedRestaurantsBreakfast.Any())
				{
					foreach (var rId in vm.SelectedRestaurantsBreakfast)
					{
						_context.Products_Restaurants.Add(new Products_Restaurants
						{
							ProductID = productId,
							RestaurantID = rId.ID,
							OrderIndex = rId.OrderIndex,
							MealType = "Breakfast"
						});
					}
				}

				// (d) 餐廳 – 午餐
				if (vm.SelectedRestaurantsLunch != null && vm.SelectedRestaurantsLunch.Any())
				{
					foreach (var rId in vm.SelectedRestaurantsLunch)
					{
						_context.Products_Restaurants.Add(new Products_Restaurants
						{
							ProductID = productId,
							RestaurantID = rId.ID,
							OrderIndex = rId.OrderIndex,
							MealType = "Lunch"
						});
					}
				}

				// (e) 餐廳 – 晚餐
				if (vm.SelectedRestaurantsDinner != null && vm.SelectedRestaurantsDinner.Any())
				{
					foreach (var rId in vm.SelectedRestaurantsDinner)
					{
						_context.Products_Restaurants.Add(new Products_Restaurants
						{
							ProductID = productId,
							RestaurantID = rId.ID,
							OrderIndex = rId.OrderIndex,
							MealType = "Dinner"
						});
					}
				}

				// (f) 景點 - 按天數存 (方法 1)
				if (vm.SelectedLocations != null && vm.SelectedLocations.Any())
				{
					foreach (var dayBlock in vm.SelectedLocations) // dayBlock = DayLocations
					{
						int dayNumber = dayBlock.DayNumber;

						foreach (var item in dayBlock.Locations)
						{
							_context.Products_Locations.Add(new Products_Locations
							{
								ProductID = productId,
								LocationID = item.ID,
								DayNumber = dayNumber,
								OrderIndex = item.OrderIndex
							});
						}
					}
				}

				_context.ProductAnalysis.Add(new ProductAnalysis
				{
					ProductID = productId,
					ReleaseDate = vm.ProductAnalysis.ReleaseDate
				});

				// ===== 3. 寫入所有關聯表 =====
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
            }

			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", vm.Product.RegionID);
			return View(vm);
        }

		// GET: ProductManagement/Products/Edit/5
		[HttpGet]
		[Route("ProductManagement/Products/Edit/{id?}")]
		public async Task<IActionResult> Edit(int id)
		{
			var product = await _context.Products
				.Include(p => p.ProductHotels)
				.Include(p => p.ProductsTransportations)
				.Include(p => p.ProductRestaurants)
				.Include(p => p.ProductLocations)
				.FirstOrDefaultAsync(p => p.ProductID == id);

			if (product == null) return NotFound();

			var vm = new ProductCreateViewModel
			{
				Product = product,

				ProductAnalysis = product.ProductAnalyses
					.Select(t => new ProductAnalysis
					{
						ProductID = t.ProductID,
						ReleaseDate = t.ReleaseDate,
						RemovalDate = t.RemovalDate
					})
					.FirstOrDefault(),

				// 已選飯店
				SelectedHotelIds = product.ProductHotels
					.OrderBy(h => h.OrderIndex)
					.Select(h => new OrderedItem { ID = h.HotelID, OrderIndex = h.OrderIndex })
					.ToList(),

				// 已選交通
				SelectedTransportationIds = product.ProductsTransportations
					.Select(t => t.TransportID.Value)
					.ToList(),

				// 已選餐廳 (依餐別)
				SelectedRestaurantsBreakfast = product.ProductRestaurants
					.Where(r => r.MealType == "Breakfast")
					.OrderBy(r => r.OrderIndex)
					.Select(r => new OrderedItem { ID = r.RestaurantID, OrderIndex = r.OrderIndex })
					.ToList(),

				SelectedRestaurantsLunch = product.ProductRestaurants
					.Where(r => r.MealType == "Lunch")
					.OrderBy(r => r.OrderIndex)
					.Select(r => new OrderedItem { ID = r.RestaurantID, OrderIndex = r.OrderIndex })
					.ToList(),

				SelectedRestaurantsDinner = product.ProductRestaurants
					.Where(r => r.MealType == "Dinner")
					.OrderBy(r => r.OrderIndex)
					.Select(r => new OrderedItem { ID = r.RestaurantID, OrderIndex = r.OrderIndex })
					.ToList(),

				// 已選景點 (依天數)
				//SelectedLocationsByDay = product.ProductLocations
				//	.GroupBy(l => (int)l.DayNumber)
				//	.ToDictionary(
				//		g => g.Key,
				//		g => g.OrderBy(l => l.OrderIndex)
				//			  .Select(l => new OrderedItem { ID = l.LocationID, OrderIndex = l.OrderIndex })
				//			  .ToList()
				//	)

				// ✅ 已選景點 (轉換成 List<DayLocations>)
				SelectedLocations = product.ProductLocations
					.GroupBy(l => l.DayNumber)
					.OrderBy(g => g.Key)
					.Select(g => new DayLocations
					{
						DayNumber = g.Key ?? 0, // DayNumber 允許 null 的話要處理
						Locations = g.OrderBy(l => l.OrderIndex)
										 .Select(l => new OrderedItem
										 {
											 ID = l.LocationID,
											 OrderIndex = l.OrderIndex
										 }).ToList()
					})
						.ToList()
			};

			ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", product.RegionID);

			return View(vm);
		}


		// POST: ProductManagement/Products/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("ProductManagement/Products/Edit/{pid?}")]
		public async Task<IActionResult> Edit(int pid, ProductCreateViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				ViewData["RegionID"] = new SelectList(_context.Regions, "RegionID", "RegionName", vm.Product.RegionID);
				return View(vm);
			}

			// 交易開始
			using var tx = await _context.Database.BeginTransactionAsync();

			try
			{
				// 1. 查產品本體
				var entity = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductID == pid);
				if (entity == null) return NotFound();

				// 2. 更新基本欄位
				entity.ProductName = vm.Product.ProductName;
				entity.ProductCode = vm.Product.ProductCode;
				entity.RegionID = vm.Product.RegionID;
				entity.ProductPrice = vm.Product.ProductPrice;
				entity.StartDate = vm.Product.StartDate;
				entity.EndDate = vm.Product.EndDate;
				entity.MaxPeople = vm.Product.MaxPeople;
				entity.ProductDesc = vm.Product.ProductDesc;
				entity.ProductNote = vm.Product.ProductNote;
				entity.IsActive = vm.Product.IsActive;

				if (vm.UploadImage != null && vm.UploadImage.Length > 0)
				{
					using var br = new BinaryReader(vm.UploadImage.OpenReadStream());
					entity.ProductImage = br.ReadBytes((int)vm.UploadImage.Length);
				}

				_context.Products.Update(entity);

				var id = entity.ProductID;

				// ================= 關聯清除 + 重建 =================

				// (a) 交通
				var oldTrans = await _context.Products_Transportations
					.Where(pt => pt.ProductID == id)
					.AsNoTracking()
					.ToListAsync();
				_context.Products_Transportations.RemoveRange(oldTrans);

				if (vm.SelectedTransportationIds?.Any() == true)
				{
					foreach (var tId in vm.SelectedTransportationIds)
					{
						_context.Products_Transportations.Add(new Products_Transportations
						{
							ProductID = id,
							TransportID = tId
						});
					}
				}

				// (b) 飯店
				var oldHotels = await _context.Products_Hotels
					.Where(ph => ph.ProductID == id)
					.AsNoTracking()
					.ToListAsync();
				_context.Products_Hotels.RemoveRange(oldHotels);

				if (vm.SelectedHotelIds?.Any() == true)
				{
					foreach (var h in vm.SelectedHotelIds)
					{
						_context.Products_Hotels.Add(new Products_Hotels
						{
							ProductID = id,
							HotelID = h.ID,
							OrderIndex = h.OrderIndex
						});
					}
				}

				// (c) 餐廳
				var oldRestaurants = await _context.Products_Restaurants
					.Where(pr => pr.ProductID == id)
					.AsNoTracking()
					.ToListAsync();
				_context.Products_Restaurants.RemoveRange(oldRestaurants);

				if (vm.SelectedRestaurantsBreakfast?.Any() == true)
				{
					foreach (var r in vm.SelectedRestaurantsBreakfast)
					{
						_context.Products_Restaurants.Add(new Products_Restaurants
						{
							ProductID = id,
							RestaurantID = r.ID,
							OrderIndex = r.OrderIndex,
							MealType = "Breakfast"
						});
					}
				}
				if (vm.SelectedRestaurantsLunch?.Any() == true)
				{
					foreach (var r in vm.SelectedRestaurantsLunch)
					{
						_context.Products_Restaurants.Add(new Products_Restaurants
						{
							ProductID = id,
							RestaurantID = r.ID,
							OrderIndex = r.OrderIndex,
							MealType = "Lunch"
						});
					}
				}
				if (vm.SelectedRestaurantsDinner?.Any() == true)
				{
					foreach (var r in vm.SelectedRestaurantsDinner)
					{
						_context.Products_Restaurants.Add(new Products_Restaurants
						{
							ProductID = id,
							RestaurantID = r.ID,
							OrderIndex = r.OrderIndex,
							MealType = "Dinner"
						});
					}
				}

				// (d) 景點
				var oldLocations = await _context.Products_Locations
					.Where(pl => pl.ProductID == id)
					.AsNoTracking()
					.ToListAsync();
				_context.Products_Locations.RemoveRange(oldLocations);

				if (vm.SelectedLocations?.Any() == true)
				{
					foreach (var day in vm.SelectedLocations)
					{
						foreach (var item in day.Locations)
						{
							_context.Products_Locations.Add(new Products_Locations
							{
								ProductID = id,
								LocationID = item.ID,
								DayNumber = day.DayNumber,
								OrderIndex = item.OrderIndex
							});
						}
					}
				}

				_context.ProductAnalysis.Add(new ProductAnalysis
				{
					ProductID = id,
					ReleaseDate = vm.ProductAnalysis.ReleaseDate
				});

				// ================= 寫入 DB =================
				var changes = await _context.SaveChangesAsync();

				// Debug: 印出 EF 追蹤變更數
				Console.WriteLine($"EF Core saved {changes} changes");
				Console.WriteLine(_context.ChangeTracker.DebugView.LongView);

				// 提交交易
				await tx.CommitAsync();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				await tx.RollbackAsync();
				Console.WriteLine("❌ Transaction rolled back: " + ex.Message);
				throw;
			}
		}


		// GET: ProductManagement/Products/Delete/5
		[HttpGet]
		[Route("ProductManagement/Products/Delete/{id?}")]
		public async Task<IActionResult> Delete(int? id)
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



		// POST: ProductManagement/Products/Delete/5
		[HttpPost, ActionName("Delete")]
		[Route("ProductManagement/Products/Delete/{id?}")]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
			var products = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == id);
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


		// 提供 DataTables 用的 API (取得Hotels內所有的資料)

		public IActionResult GetHotels()
		{
			var hotels = _context.Hotels
				.Select(h => new { hotelID = h.HotelID, hotelName = h.HotelName, hotelAddr = h.HotelAddr })
				.ToList();

			Console.WriteLine($"回傳飯店數量: {hotels.Count}");

			return Json(new { data = hotels });
		}


		public IActionResult GetLocations()
		{
			var locations = _context.Locations
				.Select(h => new { h.LocationID, h.LocationName, h.LocationAddr })
				.ToList();

			return Json(new { data = locations });
		}


		public IActionResult GetRestaurants()
		{
			var restaurants = _context.Restaurants
				.Select(h => new { h.RestaurantID, h.RestaurantName, h.RestaurantAddr })
				.ToList();

			return Json(new { data = restaurants });
		}


		public IActionResult GetTransportations()
		{
			var transportations = _context.Transportations
				.Select(h => new { h.TransportID, h.TransportName, h.TransportDesc })
				.ToList();

			return Json(new { data = transportations });
		}
	}
}
