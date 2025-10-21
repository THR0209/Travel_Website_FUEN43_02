using Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.ViewModel;

namespace Cat_Paw_Footprint.Repositories
{
	public interface ITalkMessageRepository
	{
		// 建立群組完成
		Task<TourGroups> CreateGroupAsync(GroupCreateRequestDto dto);

		// 新增會員加入群組紀錄完成
		Task<string> AddCusToGroupAsync(string GroupCode, int JoinerId, string? JoinerName);
		// 新增訪客加入群組紀錄完成
		Task<string> AddGuestToGroupAsync(string GroupCode, string? JoinerName, string DeviceId);

		// 傳訊息完成
		Task<GroupMessages> InsertMessageAsync(GroupMessageRequestDto dto);

		// 查訊息
		Task<IEnumerable<GroupMessages>> GetMessagesByGroupAsync(int groupId);

		// 上傳照片
		Task<GroupPhotos> InsertPhotoAsync(GroupPhotoRequestDto dto);

		// 查照片
		Task<IEnumerable<GroupPhotos>> GetPhotosByGroupAsync(int groupId);

		// 設集合地
		Task<GroupLocations> InsertLocationAsync(GroupLocationRequestDto dto);

		// 查集合地
		Task<GroupLocations?> GetLatestLocationAsync(int groupId);
		//根據導遊Id取得群組列表
		Task<List<TourGroups>> GetTourGroupsByGuideIdAsync(int guideId);
		Task<TourGroups?> GetGroupByIdAsync(int groupId);// 根據群組Id取得群組詳細資訊
	}
}
