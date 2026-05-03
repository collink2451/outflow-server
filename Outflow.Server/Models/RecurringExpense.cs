using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class RecurringExpense : BaseEntity
{
	public int RecurringExpenseId { get; set; }
	public int UserId { get; set; }
	public User User { get; set; } = null!;
	public int FrequencyId { get; set; }
	public Frequency Frequency { get; set; } = null!;
	public int ExpenseCategoryId { get; set; }
	public ExpenseCategory ExpenseCategory { get; set; } = null!;
	public string Description { get; set; } = "";
	public DateTime StartDate { get; set; }
	public decimal Amount { get; set; }
	public bool AutomaticRun { get; set; }
}

public class RecurringExpenseConfiguration : IEntityTypeConfiguration<RecurringExpense>
{
	public void Configure(EntityTypeBuilder<RecurringExpense> builder)
	{
		builder.HasOne(re => re.User)
			.WithMany()
			.HasForeignKey(re => re.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(re => re.Frequency)
			.WithMany()
			.HasForeignKey(re => re.FrequencyId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(re => re.ExpenseCategory)
			.WithMany()
			.HasForeignKey(re => re.ExpenseCategoryId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
