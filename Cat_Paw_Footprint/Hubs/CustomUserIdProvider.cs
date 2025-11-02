using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class CustomUserIdProvider : IUserIdProvider
{
	public string? GetUserId(HubConnectionContext connection)
	{
		// 從登入使用者的 Claims 中取得 CustomerID
		return connection.User?.FindFirst("CustomerID")?.Value;
	}
}
