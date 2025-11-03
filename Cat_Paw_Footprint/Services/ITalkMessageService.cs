using Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.ViewModel;

namespace Cat_Paw_Footprint.Services
{
	public interface ITalkMessageService
	{
		Task<GroupMessageResponseDto> SendMessageAsync(GroupMessageRequestDto dto);//發訊息
		Task<GroupPhotoResponseDto> UploadPhotoAsync(GroupPhotoRequestDto dto);//上傳照片
		Task<GroupLocationResponseDto> SetLocationAsync(GroupLocationRequestDto dto); //設定位置
		Task<IEnumerable<GroupMessages>> GetHistoryAsync(string groupCode);//取得歷史訊息
		Task<string> JoinGroupbyCustomerAsync(string GroupCode, int JoinerId, string? JoinerName);//會員加入團體
		Task<string> JoinGuestbyDeviceAsync(string groupCode, string deviceId, string? temporaryName);//遊客加入團體
		Task<List<NewHistoryAsyncDto>> GetNewHistoryAsync(string groupCode);//新版取得歷史訊息(替換上方原本取得歷史訊息)




	}
}
