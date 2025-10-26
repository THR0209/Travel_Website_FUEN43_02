using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.ViewModel;

namespace Cat_Paw_Footprint.Services
{
	public interface ITalkMessageService
	{
		Task<GroupMessageResponseDto> SendMessageAsync(GroupMessageRequestDto dto);
		Task<GroupPhotoResponseDto> UploadPhotoAsync(GroupPhotoRequestDto dto);
		Task<GroupLocationResponseDto> SetLocationAsync(GroupLocationRequestDto dto); 
		Task<IEnumerable<GroupMessages>> GetHistoryAsync(string groupCode);

	}
}
