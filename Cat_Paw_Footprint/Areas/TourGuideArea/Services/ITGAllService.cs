using Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel;
using Cat_Paw_Footprint.ViewModel;

namespace Cat_Paw_Footprint.Areas.TourGuideArea.Services
{
	public interface ITGAllService
	{
		Task<TGLoginResponseDto?> GetGuideByAccountAsync(TGLoginRequestDto dto);// 根據帳號查詢導遊
		Task<GroupCreateResponseDto> CreateGroupAsync(GroupCreateRequestDto dto);// 建群組
		#region 上傳訊息照片
		
		
		#endregion
		#region 取得群組資訊(尚未建立)
		Task<List<GroupInfoResponseDto>> GetGroupsByGuideIdAsync(int guideId);// 根據導遊Id取得群組列表
		Task<GroupDetailResponseDto?> GetGroupDetailByGroupIdAsync(int groupId);// 根據群組Id取得群組詳細資訊
		#endregion

		Task<GroupJoinResponseDto> JoinGroupAsync(GroupJoinRequestDto dto);// 加入群組
	}
}
