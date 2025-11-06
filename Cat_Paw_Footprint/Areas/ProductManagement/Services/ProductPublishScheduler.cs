using Cat_Paw_Footprint.Data;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.ProductManagement.Services
{
	/// <summary>
	/// 每隔一段時間，依 ProductAnalysis.ReleaseDate / RemovalDate 自動切換 Products.IsActive。
	/// 預設每 1 分鐘跑一次（可改用 appsettings 設定）。
	/// </summary>
	public class ProductPublishScheduler : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<ProductPublishScheduler> _logger;
		private readonly TimeSpan _interval;

		public ProductPublishScheduler(
			IServiceScopeFactory scopeFactory,
			ILogger<ProductPublishScheduler> logger,
			IConfiguration config)
		{
			_scopeFactory = scopeFactory;
			_logger = logger;

			// 允許用 appsettings 設定間隔（秒），沒填就預設 60 秒
			var seconds = config.GetValue<int?>("ProductPublishScheduler:IntervalSeconds") ?? 60;
			if (seconds < 5) seconds = 5; // 保底避免太頻繁
			_interval = TimeSpan.FromSeconds(seconds);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			// 啟動先跑一次
			await TickOnce(stoppingToken);

			// 之後固定間隔執行
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await Task.Delay(_interval, stoppingToken);
				}
				catch (TaskCanceledException) { break; }

				await TickOnce(stoppingToken);
			}
		}

		private async Task<DateTime> GetDatabaseNowAsync(webtravel2Context db, CancellationToken ct)
		{
			// 用 DB 時間（避免 Web/DB 時差）
			var conn = db.Database.GetDbConnection();
			await conn.OpenAsync(ct);
			await using var cmd = conn.CreateCommand();
			cmd.CommandText = "SELECT SYSUTCDATETIME()";
			var result = await cmd.ExecuteScalarAsync(ct);
			return Convert.ToDateTime(result);
		}

		private async Task TickOnce(CancellationToken ct)
		{
			using var scope = _scopeFactory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<webtravel2Context>();

			try
			{
				var now = await GetDatabaseNowAsync(db, ct);

				// === 上架：ReleaseDate <= now，且目前未上架 ===
				var toActivate = await db.Products
					.Where(p => p.IsActive == false)
					.Join(db.ProductAnalysis,
						  p => p.ProductID,
						  a => a.ProductID,
						  (p, a) => new { p, a })
					.Where(x => x.a.ReleaseDate != null && x.a.ReleaseDate <= now && x.a.RemovalDate > now)
					.Select(x => x.p)
					.Distinct()
					.ToListAsync(ct);

				foreach (var p in toActivate)
					p.IsActive = true;

				//// === 下架（可選）：RemovalDate < now，且目前已上架 ===
				//var toDeactivate = await db.Products
				//	.Where(p => p.IsActive == true)
				//	.Join(db.ProductAnalysis,
				//		  p => p.ProductID,
				//		  a => a.ProductID,
				//		  (p, a) => new { p, a })
				//	.Where(x => x.a.RemovalDate != null && x.a.RemovalDate < now)
				//	.Select(x => x.p)
				//	.Distinct()
				//	.ToListAsync(ct);

				//foreach (var p in toDeactivate)
				//	p.IsActive = false;

				//if (toActivate.Count + toDeactivate.Count > 0)
				//{
				//	await db.SaveChangesAsync(ct);
				//	_logger.LogInformation(
				//		"ProductPublishScheduler: Activated {A}, Deactivated {D} at {Now}",
				//		toActivate.Count, toDeactivate.Count, now);
				//}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "ProductPublishScheduler error");
			}
		}
	}
}
