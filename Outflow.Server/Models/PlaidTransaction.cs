using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class PlaidTransaction : BaseEntity
{
	public string PlaidTransactionId { get; set; } = "";
	public int UserId { get; set; }
	public User User { get; set; } = null!;
	public int PlaidConnectionId { get; set; }
	public PlaidConnection PlaidConnection { get; set; } = null!;
	public string Name { get; set; } = "";
	public DateTime Date { get; set; }
	public decimal Amount { get; set; }
}

public class PlaidTransactionConfiguration : IEntityTypeConfiguration<PlaidTransaction>
{
	public void Configure(EntityTypeBuilder<PlaidTransaction> builder)
	{
		builder.HasKey(pt => pt.PlaidTransactionId);
		builder.Property(pt => pt.PlaidTransactionId).ValueGeneratedNever();

		builder.HasOne(pt => pt.User)
			.WithMany()
			.HasForeignKey(pt => pt.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(pt => pt.PlaidConnection)
			.WithMany()
			.HasForeignKey(pt => pt.PlaidConnectionId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
