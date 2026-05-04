using System.ComponentModel.DataAnnotations;

namespace Outflow.Server.DTOs;

public record RecurringExpenseResponse(
	int RecurringExpenseId,
	int FrequencyId,
	string FrequencyName,
	int ExpenseCategoryId,
	string CategoryName,
	string Description,
	DateTime StartDate,
	DateTime NextOccurrenceDate,
	decimal Amount,
	bool AutomaticRun);

public record CreateRecurringExpenseRequest(
	[Required] int FrequencyId,
	[Required] int ExpenseCategoryId,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	[Required] DateTime StartDate,
	[Required] decimal Amount,
	[Required] bool AutomaticRun);

public record UpdateRecurringExpenseRequest(
	[Required] int FrequencyId,
	[Required] int ExpenseCategoryId,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	[Required] DateTime StartDate,
	[Required] decimal Amount,
	[Required] bool AutomaticRun);
