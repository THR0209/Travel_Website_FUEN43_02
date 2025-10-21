using NuGet.Protocol;
namespace Cat_Paw_Footprint.ViewModel
{
	public class TalkMessageDto
	{
	}
	public class GroupCreateRequestDto// 建群組
	{
		public int GuideId { get; set; }              // 建立者(導遊)ID
		public string GroupName { get; set; } = null!;// 團名
		public string? Destination { get; set; }      // 目的地(可選)
		public DateTime? Start { get; set; }          // 出發(可選)
		public DateTime? End { get; set; }            // 結束(可選)
	}

	public class GroupCreateResponseDto// 建群組回應
	{
		public bool Success { get; set; }
		public string Message { get; set; } = null!;
		public int GroupId { get; set; }              // 新群組ID
		public string GroupCode { get; set; } = null!;// 加入代碼(給遊客/其他導遊)
	}

	// 2) 發送訊息(群組聊天)
	public class GroupMessageRequestDto// 發送訊息
	{
		public int GroupId { get; set; }
		public string GroupCode { get; set; } = null!;// 加入代碼(給遊客/其他導遊)
		public string Message { get; set; }
		string? JoinerName { get; set; }
		public string? CustomerId { get; set; }// 會員ID
		public Guid? GuestId { get; set; }// 遊客ID
		public int? GuideId { get; set; }// 導遊ID

		public string SenderType { get; set; } = null!; // "Guide" / "Guest"/ "Customer"
		public int SenderId { get; set; }
		public string? Content { get; set; }            // 文字(可空，若只傳圖片)
		public string? PhotoUrl { get; set; }           // 圖片URL(可選)
	}

	public class GroupMessageResponseDto// 發送訊息回應
	{
		public bool Success { get; set; }
		public string Message { get; set; } = null!;
		public long MessageId { get; set; }
		public DateTime SentAt { get; set; }
	}

	// 3) 上傳照片（若用 Controller 接 IFormFile，Service 可用已存好的 URL）
	public class GroupPhotoRequestDto// 上傳照片
	{
		public string GroupCode { get; set; } = null!;
		public int GroupId { get; set; }// 群組Id
		public Guid? GuestId { get; set; }// 遊客ID
		public int UploaderId { get; set; }
		public string UploaderType { get; set; } = null!; // "Guide" / "Guest"/ "Customer"
		public string PhotoUrl { get; set; } = null!;     // 檔案存好後的 URL
		public double? Latitude { get; set; }             // 可選GPS
		public double? Longitude { get; set; }
	}

	public class GroupPhotoResponseDto// 上傳照片回應
	{
		public bool Success { get; set; }
		public string Message { get; set; } = null!;
		public long PhotoId { get; set; }
		public string PhotoUrl { get; set; } = null!;
		public DateTime UploadTime { get; set; }// 上傳時間
	}

	// 4) 設定集合地點
	public class GroupLocationRequestDto// 設定集合地點
	{
		public string GroupCode { get; set; } = null!;
		public int GroupId { get; set; }
		public int? GuideId { get; set; }
		public string? CustomerId { get; set; }// 會員ID
		public Guid? GuestId { get; set; }// 遊客ID
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public string SenderType { get; set; } = null!;// 發送者類型 (Guide / Customer / Guest)

		public string? Note { get; set; }                // 備註(可選)
	}

	public class GroupLocationResponseDto// 設定集合地點回應
	{
		public bool Success { get; set; }
		public string Message { get; set; } = null!;
		public long LocationId { get; set; }
		public DateTime SetTime { get; set; }
	}
	// 加入群組的回應資料
	public class GroupJoinResponseDto
	{
		public bool Success { get; set; }                // 是否成功加入
		public string Message { get; set; } = null!;     // 系統提示訊息（成功或錯誤原因）

		public int GroupId { get; set; }                 // 加入的群組 ID
		public string GroupName { get; set; } = null!;   // 群組名稱
		public string GroupCode { get; set; } = null!;   // 群組代碼（用於重新進入群組）

		public int MemberId { get; set; }                // 成員 ID（導遊或遊客）
		public string MemberType { get; set; } = null!;  // "Guide" 或 "Guest"或 "Customer"
		public string? MemberName { get; set; }          // 成員名稱（導遊姓名或遊客暱稱）

		public DateTime JoinTime { get; set; }           // 加入時間
	}
	// 加入群組的請求資料
	public class GroupJoinRequestDto
	{
		public string GroupCode { get; set; } = null!;   // 群組代碼（導遊建立群組時產生，用來辨識團）
		public int JoinerId { get; set; }                // 加入者的ID（導遊或遊客）
		public string JoinerType { get; set; } = null!;  // "Guide" 或 "Guest"或 "Customer"
		public string? JoinerName { get; set; }          // 加入者名稱（可選，顯示在群組內）
		public string? DeviceId { get; set; }          // 裝置識別碼（遊客專用，可選）
	}
	public class GroupInfoResponseDto// 根據導遊Id取得群組列表
	{
		public int GroupId { get; set; }// 群組Id
		public string GroupCode { get; set; } = null!;// 團體代碼10碼
		public string GroupName { get; set; } = null!;// 群組名稱
	}
	public class GroupMessageDetailDto// 根據群組Id取得群組詳細資訊
	{
		public string? Content { get; set; }// 訊息內容
		public DateTime SendTime { get; set; }// 發送時間
		public string SenderType { get; set; } = null!;// 發送者類型 (Customer / Guest / Guide)
		public string? CustomerId { get; set; }// 會員ID
		public Guid? GuestId { get; set; }// 遊客ID
		public int? GuideId { get; set; }// 導遊ID
		public string? FilePath { get; set; }// 照片路徑
		public decimal? Latitude { get; set; }// GPS緯度
		public decimal? Longitude { get; set; }// GPS經度
	}
	public class GroupDetailResponseDto
	{
		public int GroupId { get; set; }
		public string GroupCode { get; set; } = null!;
		public string GroupName { get; set; } = null!;
		public List<GroupMessageResponseDto> Messages { get; set; } = new();
	}
}
