using Cat_Paw_Footprint.Hubs;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Repositories;
using Cat_Paw_Footprint.ViewModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace Cat_Paw_Footprint.Services
{
	public class TalkMessageService: ITalkMessageService
	{
		private readonly ITalkMessageRepository _repo;
		private readonly IHubContext<ChatHub> _hub;
		private readonly IWebHostEnvironment _env;

		public TalkMessageService(ITalkMessageRepository repo, IHubContext<ChatHub> hub, IWebHostEnvironment env	)
		{
			_repo = repo;
			_hub = hub;
			_env = env;
		}

		public async Task<GroupMessageResponseDto> SendMessageAsync(GroupMessageRequestDto dto)// 發送群組訊息
		{
			// 1) 先寫資料庫
			var entity = await _repo.InsertMessageAsync(dto);
			Console.WriteLine($"📡 廣播中 → GroupCode={dto.GroupCode}, SenderType={dto.SenderType}, Content={dto.Content}");

			// 2) 成功後推播到 SignalR 群組
			await _hub.Clients.Group(dto.GroupCode).SendAsync("ReceiveMessage", new
			{
				UserName = entity.UserName,
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
		public async Task<GroupPhotoResponseDto> UploadPhotoAsync(GroupPhotoRequestDto dto)// 上傳群組照片
		{
			var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "groupphotos");
			Directory.CreateDirectory(uploadsFolder);

			// ⬇️ 如果有上傳的檔案，就把它存進去
			if (dto.Photo != null)
			{
				var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";
				var filePath = Path.Combine(uploadsFolder, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await dto.Photo.CopyToAsync(stream);
				}

				// 把實際檔案路徑回填到 dto
				dto.FilePath = $"/uploads/groupphotos/{fileName}";
			}

			var photo = await _repo.InsertPhotoAsync(dto);

			// ✅ 即時推播
			await _hub.Clients.Group(dto.GroupCode).SendAsync("ReceivePhoto", new
			{
				PhotoId = photo.PhotoId,
				Url = photo.FilePath,
				UploadTime = photo.UploadTime,
				username = dto.name
			   ?? (dto.UploaderType == "Guide" ? "導遊" : "匿名"), // ✅ 新增 fallback
				Latitude = dto.Latitude,
				Longitude = dto.Longitude,
				SenderType = dto.UploaderType
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
		public async Task<GroupLocationResponseDto> SetLocationAsync(GroupLocationRequestDto dto)// 設定群組集合地點
		{
			var location = await _repo.InsertLocationAsync(dto);

			// ✅ 即時推播
			await _hub.Clients.Group(dto.GroupCode).SendAsync("ReceiveLocation", new
			{
				username = dto.name ?? "匿名",
				SenderType = dto.SenderType,
				Latitude = dto.Latitude,
				Longitude = dto.Longitude,
				SendTime = location.RecordTime
			});
			Console.WriteLine($"👉 dto.name = {dto.name}");
			return new GroupLocationResponseDto
			{
				LocationId = location.LocationId,
				Success = true,
				Message = "集合地點設定成功"
			};
		}
		public async Task<IEnumerable<GroupMessages>> GetHistoryAsync(string groupCode)// 取得群組歷史訊息
		{
			return await _repo.GetHistoryByGroupCodeAsync(groupCode);
		}
		public async Task<string> JoinGroupbyCustomerAsync(string GroupCode, int JoinerId, string? JoinerName)// 會員加入團體
		{
			return await _repo.AddCusToGroupAsync(GroupCode, JoinerId, JoinerName);
		}
		public async Task<string> JoinGuestbyDeviceAsync(string groupCode, string? temporaryName, string deviceId)// 遊客加入團體
		{
			return await _repo.AddGuestToGroupAsync(groupCode, temporaryName, deviceId);
		}
		public async Task<List<NewHistoryAsyncDto>> GetNewHistoryAsync(string groupCode)// 新版取得歷史訊息(替換上方原本取得歷史訊息)
		{
			return await _repo.GetNewHistoryByGroupCodeAsync(groupCode);
		}

	}
}
