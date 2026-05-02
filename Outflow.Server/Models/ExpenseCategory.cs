using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class ExpenseCategory : BaseEntity
{
	public int ExpenseCategoryId { get; set; }
	public int UserId { get; set; }
	public User User { get; set; } = null!;
	public string Name { get; set; } = "";
}

public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
	public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
	{
		builder.HasOne(ec => ec.User)
			.WithMany()
			.HasForeignKey(ec => ec.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
