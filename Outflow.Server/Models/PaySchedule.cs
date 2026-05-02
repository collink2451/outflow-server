using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class PaySchedule : BaseEntity
{
	public int PayScheduleId { get; set; }
	public int UserId { get; set; }
	public User User { get; set; } = null!;
	public int FrequencyId { get; set; }
	public Frequency Frequency { get; set; } = null!;
	public decimal Amount { get; set; }
	public string Description { get; set; } = "";
	public DateTime StartDate { get; set; }
	public DateTime? EndDate { get; set; }
}

public class PayScheduleConfiguration : IEntityTypeConfiguration<PaySchedule>
{
	public void Configure(EntityTypeBuilder<PaySchedule> builder)
	{
		builder.HasOne(ps => ps.User)
			.WithMany()
			.HasForeignKey(ps => ps.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(ps => ps.Frequency)
			.WithMany()
			.HasForeignKey(ps => ps.FrequencyId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
