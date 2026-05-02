using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class Frequency : BaseEntity
{
	public int FrequencyId { get; set; }
	public string Name { get; set; } = "";
	public int? DaysInterval { get; set; }
	public int? MonthsInterval { get; set; }

	public DateTime GetNextOccurrence(DateTime startDate)
	{
		if (DaysInterval != null)
			return startDate.AddDays(DaysInterval.Value);
		if (MonthsInterval != null)
			return startDate.AddMonths(MonthsInterval.Value);

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
			new Frequency { FrequencyId = 2, Name = "BiWeekly", DaysInterval = 14, MonthsInterval = null, CreatedAt = seedDate, UpdatedAt = seedDate },
			new Frequency { FrequencyId = 3, Name = "Monthly", DaysInterval = null, MonthsInterval = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
			new Frequency { FrequencyId = 4, Name = "Yearly", DaysInterval = null, MonthsInterval = 12, CreatedAt = seedDate, UpdatedAt = seedDate },
			new Frequency { FrequencyId = 5, Name = "BiYearly", DaysInterval = null, MonthsInterval = 6, CreatedAt = seedDate, UpdatedAt = seedDate }
		);
	}
}
