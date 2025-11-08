using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Services
{
	/// <summary>
	/// 🚌 TdxService：負責與 TDX（交通資料流通服務平臺）進行 API 通訊
	/// 支援：
	///  - Access Token 快取與自動更新
	///  - Gzip 壓縮資料解碼
	///  - 通用的 API 取資料方法
	/// </summary>
	public class TdxService
	{
		private readonly IConfiguration _config;    // 用來讀取 appsettings.json 設定（AppID / AppKey）
		private readonly IHttpClientFactory _httpFactory;   // 用來建立 HttpClient（官方建議使用，避免資源洩漏）

		/* 用靜態欄位保存 Token（整個應用共用一份，節省 API 呼叫） */
		private static string? _cachedToken;    // 暫存取得的 Access Token
		private static DateTime _tokenExpiry = DateTime.MinValue;   // Token 過期時間（初始為最小值）
		private static readonly object _lock = new();   // 鎖定物件（避免多執行緒重複請求 Token）

		/* 建構子：注入設定檔與 HttpClient 工廠 */
		public TdxService(IConfiguration config, IHttpClientFactory httpFactory)
		{
			_config = config;
			_httpFactory = httpFactory;
		}

		/* 取得 TDX Access Token（包含快取與過期檢查機制） */
		private async Task<string> GetAccessTokenAsync()
		{
			// 若快取中的 Token 還沒過期 → 直接回傳
			if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
				return _cachedToken;

			/* 上鎖避免多執行緒同時取 Token（Thread-Safe） */
			lock (_lock)
			{
				// double check（避免剛好有人在鎖外更新了 Token）
				if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
					return _cachedToken;
			}

			/* 從設定檔取得 Client ID / Secret */
			string clientId = _config["PTX:AppID"]!;
			string clientSecret = _config["PTX:AppKey"]!;

			/* 建立 POST 表單內容（符合 OAuth 2.0 client_credentials 格式） */
			var form = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("grant_type", "client_credentials"),
				new KeyValuePair<string, string>("client_id", clientId),
				new KeyValuePair<string, string>("client_secret", clientSecret)
			});

			/* 建立 HttpClient 並發送 POST 要求取得 Token  */
			using var client = _httpFactory.CreateClient();
			var res = await client.PostAsync("https://tdx.transportdata.tw/auth/realms/TDXConnect/protocol/openid-connect/token", form);

			// 讀取回應內容（字串格式）
			var json = await res.Content.ReadAsStringAsync();

			// 若失敗 → 拋出例外，方便外層偵錯
			if (!res.IsSuccessStatusCode)
				throw new Exception($"❌ 無法取得 Access Token：{res.StatusCode} - {json}");

			/* 將 JSON 結果解析出 token 與有效秒數 */
			var tokenObj = JsonDocument.Parse(json).RootElement;
			_cachedToken = tokenObj.GetProperty("access_token").GetString(); // 實際 token
			var expiresIn = tokenObj.GetProperty("expires_in").GetInt32(); // 有效秒數（通常 3600）

			// 設定過期時間（提前 1 分鐘刷新以防意外過期）
			_tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60); // 提前1分鐘更新

			// 記錄 log（方便除錯）
			Console.WriteLine($"✅ 已取得新 Token，有效至 {_tokenExpiry:HH:mm:ss}");

			// 回傳 token
			return _cachedToken!; 
		}

		/// <summary>
		/// ✅ 通用的 API 呼叫方法（GET）
		/// 自動帶入授權 Token、支援 Gzip、回傳 JSON 字串
		/// </summary>
		public async Task<string> GetAsync(string endpoint)
		{
			// 確保有有效的 Token（會自動快取）
			var token = await GetAccessTokenAsync();

			/* 建立 Handler，支援 GZip / Deflate 壓縮解碼 */
			var handler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			// 建立 HttpClient，並在使用完後自動釋放資源
			using var client = new HttpClient(handler, disposeHandler: true);

			/* 設定必要的 HTTP 標頭 ( 設定授權標頭與接受格式 ) */
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); // 授權標頭
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); // 接受 JSON 格式
			client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")); // 支援 Gzip 壓縮

			/* 發送 GET 請求 */
			var response = await client.GetAsync(endpoint);
			var body = await response.Content.ReadAsStringAsync(); // 讀取回應內容

			/* 若失敗 → 拋出例外（顯示錯誤代碼與內容） */
			if (!response.IsSuccessStatusCode)
				throw new Exception($"TDX 查詢失敗 ({response.StatusCode})：{body}");

			return body; //成功 → 回傳 JSON 字串
		}

	}
}
