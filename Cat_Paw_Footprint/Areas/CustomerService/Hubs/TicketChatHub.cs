using Microsoft.AspNetCore.SignalR;

/// <summary>
/// SignalR Hub for ticket-based customer service chat.
/// 提供工單聊天室功能，支援加入群組和訊息廣播。
/// </summary>
public class TicketChatHub : Hub
{
	/// <summary>
	/// 讓用戶加入特定工單聊天室群組。
	/// 前端呼叫方式：connection.invoke('JoinTicketGroup', ticketId)
	/// </summary>
	/// <param name="ticketId">工單 ID</param>
	public async Task JoinTicketGroup(int ticketId)
	{
		// 依工單 ID 將連線加入 SignalR 群組，群組名稱格式為 "ticket-工單ID"
		await Groups.AddToGroupAsync(Context.ConnectionId, $"ticket-{ticketId}");
	}

	/// <summary>
	/// 傳統的群組加入方法，支援前端用 groupName 動態加入任意群組。
	/// 前端呼叫方式：connection.invoke('JoinGroup', groupName)
	/// </summary>
	/// <param name="groupName">群組名稱</param>
	public async Task JoinGroup(string groupName)
	{
		// 讓連線加入指定名稱的 SignalR 群組
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
	}

	/// <summary>
	/// 廣播訊息給該工單聊天室群組
	/// </summary>
	/// <param name="ticketId">工單 ID</param>
	/// <param name="message">訊息內容 (可為任意物件)</param>
	public async Task SendMessage(int ticketId, object message)
	{
		// 針對指定工單群組廣播訊息，前端需監聽 ReceiveMessage 事件
		await Clients.Group($"ticket-{ticketId}").SendAsync("ReceiveMessage", message);
	}

	// --------------------------------------------------------
	// 通知客戶端工單狀態變更事件
	// --------------------------------------------------------

	/// <summary>
	/// 當工單狀態變更（例如「處理中」→「已完成」）時通知客戶端。
	/// 前端需監聽 TicketStatusChanged 事件。
	/// </summary>
	/// <param name="ticketId">工單 ID</param>
	/// <param name="newStatus">新狀態名稱（如：已完成 / 處理中）</param>
	public async Task NotifyTicketStatusChanged(int ticketId, string newStatus)
	{
		await Clients.Group($"ticket-{ticketId}")
			.SendAsync("TicketStatusChanged", ticketId, newStatus);

		Console.WriteLine($"📢 工單 #{ticketId} 狀態已更新為：{newStatus}");
	}
}