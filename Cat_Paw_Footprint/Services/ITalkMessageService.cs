using Cat_Paw_Footprint.ViewModel;

namespace Cat_Paw_Footprint.Services
{
	public interface ITalkMessageService
	{
		Task<GroupMessageResponseDto> SendMessageAsync(GroupMessageRequestDto dto);
	}
}
