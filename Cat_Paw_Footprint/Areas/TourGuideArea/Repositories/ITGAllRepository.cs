using Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel;
using Cat_Paw_Footprint.ViewModel;
using Cat_Paw_Footprint.Models;
using Humanizer;
namespace Cat_Paw_Footprint.Areas.TourGuideArea.Repositories
{
	public interface ITGAllRepository
	{
		Task<TGLoginDto?> GetGuideByAccountAsync(string account);// 根據帳號查詢導遊

	}
}
