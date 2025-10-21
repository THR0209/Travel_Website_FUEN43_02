using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.ViewModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;



namespace Cat_Paw_Footprint.Repositories
{
	public class TalkMessageRepository: ITalkMessageRepository
	{
		private readonly EmployeeDbContext _db;

		public TalkMessageRepository(EmployeeDbContext db)
		{
			_db = db;
		}
		public async Task<TourGroups> CreateGroupAsync(GroupCreateRequestDto dto)// 導遊建立群組
		{
			var GroupCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
			var newGroup = new TourGroups
			{
				GroupCode = GroupCode,
				GroupName = dto.GroupName,
				Destination = dto.Destination,
				StartDate = dto.Start,
				EndDate = dto.End,
				GuideId = dto.GuideId,
				CreateTime = DateTime.UtcNow,
				Status = "Active"
			};
			_db.TourGroups.Add(newGroup);
			await _db.SaveChangesAsync();
			return newGroup;
		}
		public async Task<string> AddCusToGroupAsync(string GroupCode, int JoinerId, string? JoinerName)//會員加入群組
		{
			var group = await _db.TourGroups.FirstOrDefaultAsync(g => g.GroupCode == GroupCode);
			if (group == null)
			{
				return "此群組不存在";
			}
			var existingMember = await _db.TourGroupMembers
				.FirstOrDefaultAsync(m => m.GroupId == group.GroupId && m.CustomerId == JoinerId.ToString());
			if (existingMember != null)
			{
				return "您已經是此群組的成員";
			}
			var newMember = new TourGroupMembers
			{
				GroupId = group.GroupId,
				CustomerId = JoinerId.ToString(),
				JoinTime = DateTime.UtcNow
			};
			_db.TourGroupMembers.Add(newMember);
			await _db.SaveChangesAsync();
			return $"成功加入群組";
		}
		public async Task<string> AddGuestToGroupAsync(string GroupCode, string? JoinerName, string DeviceId)//訪客加入群組
		{
			var group = await _db.TourGroups.FirstOrDefaultAsync(g => g.GroupCode == GroupCode);
			if (group == null)
			{
				return "此群組不存在";
			}
			var existingGuest = await _db.TourGroupGuests
				.FirstOrDefaultAsync(g => g.GroupId == group.GroupId && g.DeviceId == DeviceId);
			if (existingGuest != null)
			{
				return "您已經是此群組的成員";
			}
			var newGuest = new TourGroupGuests
			{
				GroupId = group.GroupId,
				TemporaryName = JoinerName,
				DeviceId = DeviceId,
				JoinTime = DateTime.UtcNow,
				IsMember = false
			};
			_db.TourGroupGuests.Add(newGuest);
			await _db.SaveChangesAsync();
			return $"成功加入群組";
		}
		public async Task<GroupMessages> InsertMessageAsync(GroupMessageRequestDto dto)// 新增群組訊息
		{
			// 1️⃣ 找出群組
			var group = await _db.TourGroups
				.FirstOrDefaultAsync(g => g.GroupCode == dto.GroupCode);

			if (group == null)
			{
				throw new Exception("群組不存在");
			}

			// 2️⃣ 準備新訊息物件
			var message = new GroupMessages
			{
				GroupId = group.GroupId,
				SenderType = dto.SenderType,     // "Customer" / "Guest" / "Guide"
				Content = dto.Content,
				SendTime = DateTime.UtcNow       // 使用 UTC 可保持一致性
			};

			// 3️⃣ 根據發送者類型填入對應欄位
			switch (dto.SenderType)
			{
				case "Customer":
					message.CustomerId = dto.CustomerId?.ToString();
					break;

				case "Guest":
					// 這裡假設 dto.GuestId 是 Guid 或 string（你可以依情況調整）
					if (Guid.TryParse(dto.GuestId?.ToString(), out Guid guestGuid))
						message.GuestId = guestGuid;
					break;

				case "Guide":
					message.GuideId = dto.GuideId;
					break;

				default:
					throw new Exception("未知的發送者類型");
			}

			// 4️⃣ 寫入資料庫
			_db.GroupMessages.Add(message);
			await _db.SaveChangesAsync();

			// 5️⃣ 回傳已儲存的訊息
			return message;
		}
		public async Task<IEnumerable<GroupMessages>> GetMessagesByGroupAsync(int groupId)// 查訊息
		{
			return await _db.GroupMessages
				.Where(m => m.GroupId == groupId)
				.OrderBy(m => m.SendTime)
				.ToListAsync();
		}

		public async Task<GroupPhotos> InsertPhotoAsync(GroupPhotoRequestDto dto)// 上傳照片
		{
			// 1️⃣ 找出群組
			var group = await _db.TourGroups.FirstOrDefaultAsync(g => g.GroupCode == dto.GroupCode);
			if (group == null)
				throw new Exception("群組不存在");

			// 2️⃣ 建立新照片資料
			var photo = new GroupPhotos
			{
				GroupId = group.GroupId,
				UploaderType = dto.UploaderType,
				FilePath = dto.PhotoUrl, // 這裡如果是相對路徑也OK
				Latitude = (decimal?)dto.Latitude,
				Longitude = (decimal?)dto.Longitude,
				UploadTime = DateTime.UtcNow
			};

			// 3️⃣ 根據上傳者類型填欄位
			switch (dto.UploaderType)
			{
				case "Guide":
					photo.GuideId = dto.UploaderId;
					break;

				case "Customer":
					photo.CustomerId = dto.UploaderId.ToString();
					break;

				case "Guest":
					// 如果你要支援訪客，需要讓 DTO 多一個 GuestId 屬性
					if (Guid.TryParse(dto.GuestId?.ToString(), out Guid guestGuid))
						photo.GuestId = guestGuid;
					break;

				default:
					throw new Exception("未知的上傳者類型");
			}

			// 4️⃣ 寫入資料庫
			_db.GroupPhotos.Add(photo);
			await _db.SaveChangesAsync();

			return photo;
		}
		public async Task<IEnumerable<GroupPhotos>> GetPhotosByGroupAsync(int groupId)// 查照片
		{ 			
			return await _db.GroupPhotos
				.Where(p => p.GroupId == groupId)
				.OrderBy(p => p.UploadTime)
				.ToListAsync();
		}

		public async Task<GroupLocations> InsertLocationAsync(GroupLocationRequestDto dto)// 導遊設集合地或遊客發送自己位置
		{
			var location = new GroupLocations
			{
				GroupId = dto.GroupId,
				SenderType = dto.SenderType,        // ✅ 導遊設定集合地
				Latitude = (decimal)dto.Latitude,
				Longitude = (decimal)dto.Longitude,
				RecordTime = DateTime.UtcNow,// ✅ 對應資料表的 RecordTime
				Note = dto.Note
			};
			if (dto.SenderType == "Guide")
			{
				location.GuideId = dto.GuideId;     // ✅ 導遊ID
			}
			else if (dto.SenderType == "Customer")
			{
				location.CustomerId = dto.CustomerId?.ToString();// ✅ 會員ID
			}
			else if (dto.SenderType == "Guest")
			{
				if (Guid.TryParse(dto.GuestId?.ToString(), out Guid guestGuid))
					location.GuestId = guestGuid;// ✅ 遊客ID
			}

			_db.GroupLocations.Add(location);
			await _db.SaveChangesAsync();

			return location;
		}
		public async Task<GroupLocations?> GetLatestLocationAsync(int groupId)// 查集合地
		{
			return await _db.GroupLocations
				.Where(l => l.GroupId == groupId && l.SenderType == "Guide")//判斷導遊為基準
				.OrderByDescending(l => l.RecordTime)
				.FirstOrDefaultAsync();
		}
		public async Task<List<TourGroups>> GetTourGroupsByGuideIdAsync(int guideId)//根據導遊Id取得群組列表
			{
			return await _db.TourGroups
				.Where(g => g.GuideId == guideId)
				.ToListAsync();
		}
		public async Task<TourGroups?> GetGroupByIdAsync(int groupId)// 根據群組Id取得群組詳細資訊
		{
			return await _db.TourGroups
				.FirstOrDefaultAsync(g => g.GroupId == groupId);
		}

	}
}
