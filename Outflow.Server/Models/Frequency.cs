using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class Frequency : BaseEntity
{
	public int FrequencyId { get; set; }
	public string Name { get; set; } = "";
	public int? DaysInterval { get; set; }
	public int? MonthsInterval { get; set; }

	public DateTime GetNextOccurrence(DateTime startDate, DateTime from)
	{
		if (from < startDate)
			return startDate;

		if (DaysInterval != null)
		{
			int intervals = (int)((from - startDate).TotalDays / DaysInterval.Value);
			DateTime candidate = startDate.AddDays(intervals * DaysInterval.Value);
			return candidate < from
				? startDate.AddDays((intervals + 1) * DaysInterval.Value)
				: candidate;
		}

		if (MonthsInterval != null)
		{
			int monthsElapsed = (from.Year - startDate.Year) * 12 + (from.Month - startDate.Month);
			int intervals = monthsElapsed / MonthsInterval.Value;
			DateTime candidate = startDate.AddMonths(intervals * MonthsInterval.Value);
			return candidate < from
				? startDate.AddMonths((intervals + 1) * MonthsInterval.Value)
				: candidate;
		}

		throw new InvalidOperationException($"Frequency {Name} has no interval defined.");
	}
}

public class FrequencyConfiguration : IEntityTypeConfiguration<Frequency>
{
	public void Configure(EntityTypeBuilder<Frequency> builder)
	{
		DateTime seedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		builder.HasData(
			new Frequency { FrequencyId = 1, Name = "Weekly", DaysInterval = 7, MonthsInterval = null, CreatedAt = seedDate, UpdatedAt = seedDate },
			new Frequency { FrequencyId = 2, Name = "Bi-Weekly", DaysInterval = 14, MonthsInterval = null, CreatedAt = seedDate, UpdatedAt = seedDate },
			new Frequency { FrequencyId = 3, Name = "Monthly", DaysInterval = null, MonthsInterval = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
			new Frequency { FrequencyId = 4, Name = "Semi-Annually", DaysInterval = null, MonthsInterval = 6, CreatedAt = seedDate, UpdatedAt = seedDate },
			new Frequency { FrequencyId = 5, Name = "Anually", DaysInterval = null, MonthsInterval = 12, CreatedAt = seedDate, UpdatedAt = seedDate }
		);
	}
}
