using Microsoft.AspNetCore.SignalR;

/// <summary>
/// SignalR Hub for ticket-based customer service chat.
/// 提供工單聊天室功能，支援加入群組和訊息廣播。
/// </summary>
public class TicketChatHub : Hub
{
	/// <summary>
	/// 讓用戶加入特定工單聊天室群組。
	/// </summary>
	/// <param name="ticketId">工單 ID</param>
	public async Task JoinTicketGroup(int ticketId)
	{
		// 依工單 ID 將連線加入 SignalR 群組
		await Groups.AddToGroupAsync(Context.ConnectionId, $"ticket-{ticketId}");
	}

	/// <summary>
	/// 廣播訊息給該工單聊天室群組。
	/// </summary>
	/// <param name="ticketId">工單 ID</param>
	/// <param name="message">訊息內容 (可為任意物件)</param>
	public async Task SendMessage(int ticketId, object message)
	{
		// 針對指定工單群組廣播訊息，前端需監聽 ReceiveMessage 事件
		await Clients.Group($"ticket-{ticketId}").SendAsync("ReceiveMessage", message);
	}
}