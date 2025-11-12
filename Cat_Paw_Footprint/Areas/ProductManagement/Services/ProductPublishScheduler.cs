using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Cat_Paw_Footprint.Data;

namespace Cat_Paw_Footprint.Areas.ProductManagement.Services
{
	/// <summary>
	/// 依 ProductAnalysis.ReleaseDate / RemovalDate 自動切換 Products.IsActive。
	/// 以「分鐘」為判斷單位（忽略秒與毫秒）。
	/// 預設每 1 分鐘跑一次（可用 appsettings 覆寫 ProductPublishScheduler:IntervalSeconds）。
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

			var seconds = config.GetValue<int?>("ProductPublishScheduler:IntervalSeconds") ?? 60;
			if (seconds < 5) seconds = 5; // 避免過於頻繁
			_interval = TimeSpan.FromSeconds(seconds);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			// 啟動先跑一次
			await TickOnce(stoppingToken);

			while (!stoppingToken.IsCancellationRequested)
			{
				try { await Task.Delay(_interval, stoppingToken); }
				catch (TaskCanceledException) { break; }

				await TickOnce(stoppingToken);
			}
		}

		private async Task<DateTime> GetDatabaseUtcNowAsync(webtravel2Context db, CancellationToken ct)
		{
			// 一律使用 DB 的 UTC 時間，避免主機/DB 時區差異
			var conn = db.Database.GetDbConnection();
			await conn.OpenAsync(ct);
			await using var cmd = conn.CreateCommand();
			cmd.CommandText = "SELECT SYSUTCDATETIME()";
			var result = await cmd.ExecuteScalarAsync(ct);
			return Convert.ToDateTime(result); // Kind=Unspecified，但值是 UTC
		}

		private static DateTime TruncateToMinuteUtc(DateTime dtUtc)
		{
			// 將時間截到「分鐘」：忽略秒與毫秒
			return new DateTime(dtUtc.Year, dtUtc.Month, dtUtc.Day, dtUtc.Hour, dtUtc.Minute, 0, DateTimeKind.Utc);
		}

		private async Task TickOnce(CancellationToken ct)
		{
			using var scope = _scopeFactory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<webtravel2Context>();

			try
			{
				var nowUtc = await GetDatabaseUtcNowAsync(db, ct);
				var nowMinuteUtc = TruncateToMinuteUtc(nowUtc);

				// === 以分鐘為單位：上架條件 ===
				// IsActive != true（包含 false / NULL），且 ReleaseDate <= now(min)
				// 並且 RemovalDate 為 NULL 或 > now(min)
				var toActivate = await db.Products
					.Where(p => p.IsActive != true)
					.Join(db.ProductAnalysis,
						  p => p.ProductID,
						  a => a.ProductID,
						  (p, a) => new { p, a })
					.Where(x =>
						x.a.ReleaseDate != null &&
						x.a.ReleaseDate <= nowMinuteUtc &&
						(x.a.RemovalDate == null || x.a.RemovalDate > nowMinuteUtc))
					.Select(x => x.p)
					.Distinct()
					.ToListAsync(ct);

				foreach (var p in toActivate)
					p.IsActive = true;

				// === 以分鐘為單位：下架條件（需要時保留；不需要可註解整段） ===
				var toDeactivate = await db.Products
					.Where(p => p.IsActive == true)
					.Join(db.ProductAnalysis,
						  p => p.ProductID,
						  a => a.ProductID,
						  (p, a) => new { p, a })
					.Where(x => x.a.RemovalDate != null &&
								x.a.RemovalDate <= nowMinuteUtc)
					.Select(x => x.p)
					.Distinct()
					.ToListAsync(ct);

				//foreach (var p in toDeactivate)
				//	p.IsActive = false;

				var changed = toActivate.Count + toDeactivate.Count;
				if (changed > 0)
				{
					await db.SaveChangesAsync(ct);
					_logger.LogInformation(
						"ProductPublishScheduler(minute-based): Activated {A}, Deactivated {D} at {NowUtc:O}",
						toActivate.Count, toDeactivate.Count, nowMinuteUtc);
				}
				else
				{
					_logger.LogDebug("ProductPublishScheduler(minute-based): No changes at {NowUtc:O}", nowMinuteUtc);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "ProductPublishScheduler error");
			}
		}
	}
}
