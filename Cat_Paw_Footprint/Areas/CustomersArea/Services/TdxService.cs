using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Services
{
	public class TdxService
	{
		private readonly IConfiguration _config;
		private readonly IHttpClientFactory _httpFactory;

		private static string? _cachedToken;
		private static DateTime _tokenExpiry = DateTime.MinValue;
		private static readonly object _lock = new();

		public TdxService(IConfiguration config, IHttpClientFactory httpFactory)
		{
			_config = config;
			_httpFactory = httpFactory;
		}

		// ✅ 取得 Access Token（含快取機制）
		private async Task<string> GetAccessTokenAsync()
		{
			// 若快取中的 token 未過期 → 直接回傳
			if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
				return _cachedToken;

			lock (_lock)
			{
				// double check（避免多執行緒重取 token）
				if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
					return _cachedToken;
			}

			string clientId = _config["PTX:AppID"]!;
			string clientSecret = _config["PTX:AppKey"]!;

			var form = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("grant_type", "client_credentials"),
				new KeyValuePair<string, string>("client_id", clientId),
				new KeyValuePair<string, string>("client_secret", clientSecret)
			});

			using var client = _httpFactory.CreateClient();
			var res = await client.PostAsync("https://tdx.transportdata.tw/auth/realms/TDXConnect/protocol/openid-connect/token", form);
			var json = await res.Content.ReadAsStringAsync();

			if (!res.IsSuccessStatusCode)
				throw new Exception($"❌ 無法取得 Access Token：{res.StatusCode} - {json}");

			var tokenObj = JsonDocument.Parse(json).RootElement;
			_cachedToken = tokenObj.GetProperty("access_token").GetString();
			var expiresIn = tokenObj.GetProperty("expires_in").GetInt32();

			_tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60); // 提前1分鐘更新

			Console.WriteLine($"✅ 已取得新 Token，有效至 {_tokenExpiry:HH:mm:ss}");

			return _cachedToken!;
		}

		// ✅ 呼叫 TDX API 的通用方法
		public async Task<string> GetAsync(string endpoint)
		{
			var token = await GetAccessTokenAsync();

			// ✅ 用 handler 來建立 HttpClient
			var handler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			using var client = new HttpClient(handler, disposeHandler: true);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

			var response = await client.GetAsync(endpoint);
			var body = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
				throw new Exception($"TDX 查詢失敗 ({response.StatusCode})：{body}");

			return body;
		}

	}
}
