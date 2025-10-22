using Cat_Paw_Footprint.Hubs;
using Cat_Paw_Footprint.Repositories;
using Cat_Paw_Footprint.ViewModel;
using Microsoft.AspNetCore.SignalR;

namespace Cat_Paw_Footprint.Services
{
	public class TalkMessageService: ITalkMessageService
	{
		private readonly ITalkMessageRepository _repo;
		private readonly IHubContext<ChatHub> _hub;

		public TalkMessageService(ITalkMessageRepository repo, IHubContext<ChatHub> hub)
		{
			_repo = repo;
			_hub = hub;
		}

		public async Task<GroupMessageResponseDto> SendMessageAsync(GroupMessageRequestDto dto)
		{
			// 1) 先寫資料庫
			var entity = await _repo.InsertMessageAsync(dto);

			// 2) 成功後推播到 SignalR 群組
			await _hub.Clients.Group(dto.GroupCode).SendAsync("ReceiveMessage", new
			{
				SenderType = dto.SenderType,
				Content = entity.Content,
				SendTime = entity.SendTime
			});

			// 3) 回傳結果 DTO（給 Controller 用）
			return new GroupMessageResponseDto
			{
				MessageId = entity.MessageId,
				SentAt = entity.SendTime,
				Success = true,
				Message = "訊息發送成功"
			};
		}
		public async Task<GroupPhotoResponseDto> UploadPhotoAsync(GroupPhotoRequestDto dto)
		{
			var photo = await _repo.InsertPhotoAsync(dto);

			// ✅ 即時推播
			await _hub.Clients.Group(dto.GroupCode).SendAsync("ReceivePhoto", new
			{
				PhotoId = photo.PhotoId,
				Url = photo.FilePath,
				UploadTime = photo.UploadTime
			});

			return new GroupPhotoResponseDto
			{
				PhotoId = photo.PhotoId,
				PhotoUrl = photo.FilePath,
				UploadTime = photo.UploadTime,
				Success = true,
				Message = "照片上傳成功"
			};
		}
		public async Task<GroupLocationResponseDto> SetLocationAsync(GroupLocationRequestDto dto)
		{
			var location = await _repo.InsertLocationAsync(dto);

			// ✅ 即時推播
			await _hub.Clients.Group(dto.GroupCode).SendAsync("ReceiveLocation", new
			{
				Latitude = location.Latitude,
				Longitude = location.Longitude,
				SetTime = location.RecordTime
			});

			return new GroupLocationResponseDto
			{
				LocationId = location.LocationId,
				Success = true,
				Message = "集合地點設定成功"
			};
		}

	}
}
