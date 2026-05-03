using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.Models;

namespace Outflow.Server.Services;

public class RecurringExpenseService(IServiceScopeFactory scopeFactory, ILogger<RecurringExpenseService> logger) : BackgroundService
{
	const string SERVICE_KEY = "RecurringExpenseService";
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

			List<RecurringExpense> recurringExpenses = await db.RecurringExpenses
				.Where(re => re.AutomaticRun && re.StartDate <= today)
				.Include(re => re.Frequency)
				.ToListAsync(stoppingToken);

			foreach (RecurringExpense recurringExpense in recurringExpenses)
			{
				DateTime next = recurringExpense.Frequency.GetNextOccurrence(recurringExpense.StartDate, today);
				if (next.Date != today)
					continue;

				db.Expenses.Add(new Expense
				{
					UserId = recurringExpense.UserId,
					ExpenseCategoryId = recurringExpense.ExpenseCategoryId,
					Description = recurringExpense.Description,
					Date = today,
					Amount = recurringExpense.Amount
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
			logger.LogError(ex, "Error during recurring expense processing");
		}
	}
}
