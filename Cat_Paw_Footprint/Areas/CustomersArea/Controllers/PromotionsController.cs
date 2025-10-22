using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
    [Area("CustomersArea")]
	[AllowAnonymous]
	public class PromotionsController : Controller
    {
        private readonly webtravel2Context _context;

        public PromotionsController(webtravel2Context context)
        {
            _context = context;
        }

        // GET: CustomersArea/Promotions
        public async Task<IActionResult> Index()
        {
            var activePromotions = await _context.Promotions.Where
                (p=> p.IsActive && 
                 p.StartTime <= DateTime.Now && 
                 p.EndTime >= DateTime.Now).OrderByDescending(p=> p.StartTime).ToListAsync();

			return View(activePromotions);
        }

        // GET: CustomersArea/Promotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotions = await _context.Promotions
                .FirstOrDefaultAsync(m => m.PromoID == id);
            if (promotions == null)
            {
                return NotFound();
            }

            return View(promotions);
        }

		// 新增：給 Vue.js 撈資料的 JSON Action
		[HttpGet]
		public async Task<IActionResult> GetActivePromotions()
		{
			var promos = await _context.Promotions
				//.Where(p => p.IsActive &&
				//			p.StartTime <= DateTime.Now &&
				//			p.EndTime >= DateTime.Now)
				//.OrderByDescending(p => p.StartTime)
				//.Select(p => new
				//{
				//	p.PromoID,
				//	p.PromoName,
				////	p.ImageUrl,
    //                p.StartTime
				//})
                .Where(p => p.IsActive)
				.ToListAsync();

			return Json(promos);
		}

		[HttpGet]
		public async Task<IActionResult> GetLatestPromotions()
		{
			var promos = await _context.Promotions
				.Where(p => p.IsActive)
				//.OrderByDescending(p => p.StartTime)
				.Take(3)
				.ToListAsync();

			return Json(promos);
		}

		private bool PromotionsExists(int id)
        {
            return _context.Promotions.Any(e => e.PromoID == id);
        }
    }
}
