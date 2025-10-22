using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Controllers
{
	[Area("CustomersArea")]
	[AllowAnonymous] // 首頁允許匿名訪問
	public class HomeController : Controller
	{
		/// <summary>
		/// 首頁：訪客與會員皆可瀏覽，不強制跳轉
		/// </summary>
		public IActionResult Index()
		{
			// 直接回傳首頁 View，不論是否登入
			return View();
		}
	}
}