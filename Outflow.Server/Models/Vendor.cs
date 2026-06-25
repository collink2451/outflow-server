using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class Vendor : BaseEntity
{
	public int VendorId { get; set; }
	public int UserId { get; set; }
	public User User { get; set; } = null!;
	public int? ExpenseCategoryId { get; set; }
	public ExpenseCategory? ExpenseCategory { get; set; }
	public string Name { get; set; } = "";
	public string MatchPattern { get; set; } = "";
	public bool AutoDismiss { get; set; }
}

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
	public void Configure(EntityTypeBuilder<Vendor> builder)
	{
		builder.HasOne(v => v.User)
			.WithMany()
			.HasForeignKey(v => v.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(v => v.ExpenseCategory)
			.WithMany()
			.HasForeignKey(v => v.ExpenseCategoryId)
			.IsRequired(false)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
