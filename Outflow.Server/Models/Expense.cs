using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class Expense : BaseEntity
{
	public int ExpenseId { get; set; }
	public int UserId { get; set; }
	public User User { get; set; } = null!;
	public int ExpenseCategoryId { get; set; }
	public ExpenseCategory ExpenseCategory { get; set; } = null!;
	public string Description { get; set; } = "";
	public DateTime Date { get; set; }
	public decimal Amount { get; set; }
}

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
	public void Configure(EntityTypeBuilder<Expense> builder)
	{
		builder.HasOne(e => e.User)
			.WithMany()
			.HasForeignKey(e => e.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(e => e.ExpenseCategory)
			.WithMany()
			.HasForeignKey(e => e.ExpenseCategoryId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasIndex(e => new { e.UserId, e.Date });
	}
}
