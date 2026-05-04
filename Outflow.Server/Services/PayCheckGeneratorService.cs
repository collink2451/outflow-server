using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.Models;

namespace Outflow.Server.Services;

public class PayCheckGeneratorService(IServiceScopeFactory scopeFactory, ILogger<PayCheckGeneratorService> logger) : BackgroundService
{
	const string SERVICE_KEY = "PayCheckGeneratorService";

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await RunAsync(stoppingToken);

		while (!stoppingToken.IsCancellationRequested)
		{
			DateTime next8am = DateTime.UtcNow.Date.AddDays(1).AddHours(8);
			await Task.Delay(next8am - DateTime.UtcNow, stoppingToken);
			await RunAsync(stoppingToken);
		}
	}

	private async Task RunAsync(CancellationToken stoppingToken)
	{
		try
		{
			using IServiceScope scope = scopeFactory.CreateScope();
			AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

			ServiceTimestamp? ts = await db.ServiceTimestamps.FindAsync([SERVICE_KEY], stoppingToken);
			if (ts != null && ts.LastRunAt.Date == DateTime.UtcNow.Date)
				return;

			DateTime today = DateTime.UtcNow.Date;

			List<PaySchedule> schedules = await db.PaySchedules
				.Where(ps => ps.StartDate <= today && (ps.EndDate == null || ps.EndDate >= today))
				.Include(ps => ps.Frequency)
				.ToListAsync(stoppingToken);

			foreach (PaySchedule schedule in schedules)
			{
				DateTime next = schedule.Frequency.GetNextOccurrence(schedule.StartDate, today);
				if (next.Date != today)
					continue;

				bool exists = await db.PayChecks.AnyAsync(
					pc => pc.UserId == schedule.UserId && pc.Date == today,
					stoppingToken);

				if (exists)
					continue;

				db.PayChecks.Add(new PayCheck
				{
					UserId = schedule.UserId,
					Amount = schedule.Amount,
					Date = today,
				});
			}

			if (ts == null)
				db.ServiceTimestamps.Add(new ServiceTimestamp { ServiceName = SERVICE_KEY, LastRunAt = DateTime.UtcNow });
			else
				ts.LastRunAt = DateTime.UtcNow;

			await db.SaveChangesAsync(stoppingToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error during pay check generation");
		}
	}
}
