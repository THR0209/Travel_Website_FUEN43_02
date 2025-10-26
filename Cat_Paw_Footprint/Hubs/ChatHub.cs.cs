using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;
namespace Cat_Paw_Footprint.Hubs
{
	public class ChatHub: Hub
	{
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
		public async Task SendMessageToGroup(string groupCode, string senderType, string content)
		{
			
			var message = new
			{
				SenderType = senderType,
				Content = content,
				SendTime = DateTime.UtcNow
			};

			await Clients.Group(groupCode).SendAsync("ReceiveMessage", message);
			await Clients.Caller.SendAsync("ReceiveMessage", message); // ✅ 自己也收到
		}
	}
}
