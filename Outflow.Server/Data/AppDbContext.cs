using Microsoft.EntityFrameworkCore;
using Outflow.Server.Models;
using System.Reflection;

namespace Outflow.Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	public DbSet<Frequency> Frequencies { get; set; }
	public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
	public DbSet<Expense> Expenses { get; set; }
	public DbSet<PayCheck> PayChecks { get; set; }
	public DbSet<PaySchedule> PaySchedules { get; set; }
	public DbSet<RecurringExpense> RecurringExpenses { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		var now = DateTime.UtcNow;

		foreach (var entry in ChangeTracker.Entries<BaseEntity>())
		{
			if (entry.State == EntityState.Added)
			{
				entry.Entity.CreatedAt = now;
				entry.Entity.UpdatedAt = now;
			}
			else if (entry.State == EntityState.Modified)
			{
				entry.Entity.UpdatedAt = now;
			}
		}

		return await base.SaveChangesAsync(cancellationToken);
	}
}
