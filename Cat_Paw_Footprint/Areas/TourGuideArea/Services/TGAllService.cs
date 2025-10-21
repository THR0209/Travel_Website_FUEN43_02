using Cat_Paw_Footprint.Areas.TourGuideArea.ViewModel;
using Cat_Paw_Footprint.ViewModel;
using Cat_Paw_Footprint.Areas.TourGuideArea.Repositories;
using Cat_Paw_Footprint.Repositories;
namespace Cat_Paw_Footprint.Areas.TourGuideArea.Services
{
	public class TGAllService: ITGAllService
	{
		private readonly ITGAllRepository _TGRepo;
		private readonly ITalkMessageRepository _repo;
		public TGAllService(ITGAllRepository TGRepo, ITalkMessageRepository tgAllRepository)
		{
			_TGRepo = TGRepo;
			_repo = tgAllRepository;
		}
		public async Task<TGLoginResponseDto?> GetGuideByAccountAsync(TGLoginRequestDto dto)// 根據帳號查詢導遊
		{
			var account = dto.Account;
			var guide = await _TGRepo.GetGuideByAccountAsync(account);// 根據帳號查詢導遊
			if (guide == null)
			{
				return new TGLoginResponseDto
				{
					Success = false,
					Message = "帳號不存在"
				};
			}
			bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, guide.Password);
			
			if (isPasswordValid == false)
			{
				return new TGLoginResponseDto
				{
					Success = false,
					Message = "密碼錯誤"
				}; // 密碼錯誤
			}
			if (guide.Status == false)
			{
				return new TGLoginResponseDto
				{
					Success = false,
					Message = "帳號已被停用，請聯絡管理員"
				}; // 帳號被停用
			}
			if (guide.RoleID != 2) 
			{
				return new TGLoginResponseDto
				{
					Success = false,
					Message = "非導遊帳號，請使用正確的帳號登入"
				}; // 非導遊帳號
			}
			// 登入成功
			return new TGLoginResponseDto
			{
				Account = guide.Account,
				Success = true,
				Message = "登入成功",
				GuideName = guide.EmployeeName,
				Token = null // 預留擴充用
			};
		}

		public async Task<GroupCreateResponseDto> CreateGroupAsync(GroupCreateRequestDto dto)// 建群組(完成)
		{
			var group = await _repo.CreateGroupAsync(dto);
			return new GroupCreateResponseDto
			{
				GroupId = group.GroupId,// 新群組ID
				Success = true,
				Message = "群組建立成功"
			};
		}
		public async Task<GroupJoinResponseDto> JoinGroupAsync(GroupJoinRequestDto dto)//加入群組(完成)
		{
			string message = "成功加入群組";
			bool success = true;
			if (dto.JoinerType == "Customer")// 如果是客戶的話就加入群組
			{
				message=await _repo.AddCusToGroupAsync(dto.GroupCode, dto.JoinerId, dto.JoinerName);
				if(message!="成功加入群組")
				{
					success=false;
				}
			}
			else if(dto.JoinerType == "Guest")// 如果是遊客的話就直接加入群組
			{
				message=await _repo.AddGuestToGroupAsync(dto.GroupCode, dto.JoinerName, dto.DeviceId);
				if(message!="成功加入群組")
				{
					success=false;
				}
			}
			else
			{
				success = false;
				message = "加入群組失敗，無效的加入者類型";
			}
			
			return new GroupJoinResponseDto
			{
				Success = success,
				Message = message
			};
		}

		
		
		public async Task<List<GroupInfoResponseDto>> GetGroupsByGuideIdAsync(int guideId)//根據導遊Id取得群組列表
		{
			var allGroups = await _repo.GetTourGroupsByGuideIdAsync(guideId);

			return allGroups.Select(g => new GroupInfoResponseDto
			{
				GroupId = g.GroupId,
				GroupCode = g.GroupCode,
				GroupName = g.GroupName
			}).ToList();
		}
		public async Task<GroupDetailResponseDto?> GetGroupDetailByGroupIdAsync(int groupId)// 根據群組Id取得群組詳細資訊可能未來用到)
		{
			var group = await _repo.GetGroupByIdAsync(groupId);
			if (group == null)
				return null;

			// 查訊息
			var messages = await _repo.GetMessagesByGroupAsync(groupId);

			// 組成回傳資料
			var result = new GroupDetailResponseDto
			{
				GroupId = group.GroupId,
				GroupCode = group.GroupCode,
				GroupName = group.GroupName,
				Messages = messages.Select(m => new GroupMessageResponseDto
				{
					MessageId = m.MessageId,
					SentAt = m.SendTime,     // ✅ 注意拼字
					Success = true,
					Message = m.Content      // ✅ 注意你的資料表欄位名稱（如果叫 MessageContent 就改成那個）
				}).ToList()
			};

			return result;
		}
		
		

	}
}
