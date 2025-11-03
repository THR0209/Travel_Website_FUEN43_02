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
            return View();
        }

        // 🟢 2️⃣ Vue 用的 API：顯示所有優惠活動
        [HttpGet]
        public async Task<IActionResult> GetAllActivePromotions()
        {
            var promos = await _context.Promotions
                .Where(p => p.IsActive) // 只撈啟用的
                .OrderByDescending(p => p.StartTime)
                .Select(p => new
                {
                    p.PromoID,
                    p.PromoName,
                    p.PromoDesc,
                    p.StartTime,
                    p.EndTime,
                    ImageUrl = "/images/NoImage.png" // 統一預設圖
                })
                .ToListAsync();

            return Json(promos);
        }

        // GET: CustomersArea/Promotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
           // var promo = await _context.Promotions
           //.Where(p => p.PromoID == id && p.IsActive)
           //.Select(p => new
           //{
           //    p.PromoID,
           //    p.PromoName,
           //    p.PromoDesc,
           //    p.StartTime,
           //    p.EndTime,
           //    ImageUrl = "/images/NoImage.png"
           //})
           //.FirstOrDefaultAsync();

           // if (promo == null)
           //     return NotFound();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var promo = await _context.Promotions
                .Where(p => p.PromoID == id && p.IsActive)
                .Select(p => new
                {
                    p.PromoID,
                    p.PromoName,
                    p.PromoDesc,
                    p.StartTime,
                    p.EndTime,
                    p.DiscountType,
                    p.DiscountValue,
                    ImageUrl = "/images/NoImage.png"
                })
                .FirstOrDefaultAsync();

            if (promo == null)
                return NotFound();

            return Json(promo);
        }




        [HttpGet]
        public async Task<IActionResult> GetLatestPromotions()
        {
            var promos = await _context.Promotions
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.StartTime)
                .Take(5)
                .Select(p => new
                {
                    p.PromoID,
                    p.PromoName,
                    p.PromoDesc,
                    p.StartTime,
                    p.EndTime,
                   // ImageUrl = string.IsNullOrEmpty(p.ImageUrl) ? "/images/NoImage.png" : p.ImageUrl
                    ImageUrl = "/images/NoImage.png" // 固定給一張
                })
                .ToListAsync();

            return Json(promos);
        }


        private bool PromotionsExists(int id)
        {
            return _context.Promotions.Any(e => e.PromoID == id);
        }
    }
}
