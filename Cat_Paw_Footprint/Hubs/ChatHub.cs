using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace Cat_Paw_Footprint.Hubs
{
	public class ChatHub: Hub
	{
		private readonly EmployeeDbContext _db;
		public ChatHub(EmployeeDbContext db)
		{
			_db = db;
		}
		// 使用者加入群組
		public async Task JoinGroup(string groupCode)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, groupCode);
			Console.WriteLine($"✅ {Context.ConnectionId} 已加入群組 {groupCode}");
		}

		// 離開群組
		public async Task LeaveGroup(string groupCode)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupCode);
			Console.WriteLine($"✅ {Context.ConnectionId} 已加入群組 {groupCode}");
		}

		// 傳送訊息給該群組所有人
		public async Task SendMessageToGroup(string groupCode, string senderType, string content, string? deviceId = null, int? customerId = null)
		{

			string userName = "匿名";

			// 🧩 根據 senderType 查名稱
			if (senderType == "Customer" && customerId.HasValue)
			{
				userName = await _db.Customers
					.Where(c => c.CustomerID == customerId.Value)
					.Select(c => c.CustomerProfile.CustomerName)
					.FirstOrDefaultAsync() ?? "會員";
			}
			else if (senderType == "Guest" && !string.IsNullOrEmpty(deviceId))
			{
				userName = await _db.TourGroupGuests
					.Where(g => g.DeviceId == deviceId)
					.Select(g => g.TemporaryName)
					.FirstOrDefaultAsync() ?? "訪客";
			}

			var message = new
			{
				SenderType = senderType,
				UserName = userName,   // ✅ 真實名稱
				Content = content,
				SendTime = DateTime.UtcNow
			};

			await Clients.Group(groupCode).SendAsync("ReceiveMessage", message);// ✅ 傳給群組其他人
			await Clients.Caller.SendAsync("ReceiveMessage", message); // ✅ 自己也收到
		}
	}
}
