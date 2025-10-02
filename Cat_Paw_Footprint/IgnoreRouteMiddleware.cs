
// 防止對 .aspx 頁面的請求被處理
public class IgnoreRouteMiddleware 
{
	private readonly RequestDelegate next;
	public IgnoreRouteMiddleware(RequestDelegate next)
	{
		this.next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		if (context.Request.Path.HasValue &&
			context.Request.Path.Value.EndsWith("aspx")) 
		{
			return; // Ignore .aspx requests
		}
		await next.Invoke(context);
	}

}