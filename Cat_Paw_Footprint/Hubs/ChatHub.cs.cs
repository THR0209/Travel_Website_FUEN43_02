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
		}

		// 離開群組
		public async Task LeaveGroup(string groupCode)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupCode);
		}

		// 傳送訊息給該群組所有人
		public async Task SendMessageToGroup(string groupCode, string senderType, string content)
		{
			await Clients.Group(groupCode).SendAsync("ReceiveMessage", new
			{
				SenderType = senderType,
				Content = content,
				SendTime = DateTime.Now
			});
		}
	}
}
