using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Cat_Paw_Footprint.Data;

public class CleanupOptions
{
	public int KeepPendingDays { get; set; } = 3;
	public int KeepPaidDays { get; set; } = 30;
	public string RunAt { get; set; } = "03:10";
}

public class PendingPaymentsCleanupService : BackgroundService
{
	private readonly IServiceProvider _sp;
	private readonly ILogger<PendingPaymentsCleanupService> _logger;
	private readonly CleanupOptions _opt;

	public PendingPaymentsCleanupService(
		IServiceProvider sp,
		IOptions<CleanupOptions> opt,
		ILogger<PendingPaymentsCleanupService> logger)
	{
		_sp = sp;
		_logger = logger;
		_opt = opt.Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var delay = NextDelay(_opt.RunAt);
			await Task.Delay(delay, stoppingToken);

			try
			{
				using var scope = _sp.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<webtravel2Context>();

				var now = DateTime.Now;
				var pendingCut = now.AddDays(-_opt.KeepPendingDays);
				var paidCut = now.AddDays(-_opt.KeepPaidDays);

				// 分批刪，避免長交易
				int batch, total = 0;

				do
				{
					batch = await db.PendingPayments
						.Where(p => p.Status == 0 && p.CreatedAt < pendingCut)
						.OrderBy(p => p.PendingPaymentId)
						.Take(1000)
						.ExecuteDeleteAsync(stoppingToken); // EF Core 7+ 可用；舊版改成 ToList + RemoveRange

					total += batch;
				} while (batch > 0 && !stoppingToken.IsCancellationRequested);

				// 保險：清已付款超過保留期（理論上已刪）
				int paidDel = await db.PendingPayments
					.Where(p => p.Status == 1 && p.CreatedAt < paidCut)
					.ExecuteDeleteAsync(stoppingToken);

				_logger.LogInformation("Cleanup done. pending deleted: {pdel}, paid deleted: {paidDel}", total, paidDel);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "PendingPayments cleanup error");
			}
		}
	}

	private static TimeSpan NextDelay(string runAt)
	{
		// runAt 格式 "HH:mm"
		var parts = runAt.Split(':');
		var hh = int.Parse(parts[0]);
		var mm = int.Parse(parts[1]);

		var now = DateTime.Now;
		var next = new DateTime(now.Year, now.Month, now.Day, hh, mm, 0);
		if (next <= now) next = next.AddDays(1);
		return next - now;
	}
}
