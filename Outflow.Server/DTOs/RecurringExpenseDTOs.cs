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
	int FrequencyId,
	int ExpenseCategoryId,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	DateTime StartDate,
	decimal Amount,
	bool AutomaticRun);

public record UpdateRecurringExpenseRequest(
	int FrequencyId,
	int ExpenseCategoryId,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	DateTime StartDate,
	decimal Amount,
	bool AutomaticRun);
