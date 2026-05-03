using Outflow.Server.Data;

namespace Outflow.Server.Services;

public class DemoResetService(IServiceScopeFactory scopeFactory, ILogger<DemoResetService> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			DateTime now = DateTime.UtcNow;
			DateTime tomorrow8am = now.Date.AddDays(1).AddHours(8);
			TimeSpan delay = tomorrow8am - now;
			await Task.Delay(delay, stoppingToken);

			try
			{
				using IServiceScope scope = scopeFactory.CreateScope();
				AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
				await DemoDataSeeder.SeedAsync(db);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error during demo reset");
			}
		}
	}
}
