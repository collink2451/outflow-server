using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class PayCheck : BaseEntity
{
	public int PayCheckId { get; set; }
	public int UserId { get; set; }
	public User User { get; set; } = null!;
	public decimal Amount { get; set; }
	public DateTime Date { get; set; }
}

public class PayCheckConfiguration : IEntityTypeConfiguration<PayCheck>
{
	public void Configure(EntityTypeBuilder<PayCheck> builder)
	{
		builder.HasOne(pc => pc.User)
			.WithMany()
			.HasForeignKey(pc => pc.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(pc => new { pc.UserId, pc.Date });
	}
}
