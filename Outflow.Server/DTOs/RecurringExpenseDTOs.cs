namespace Outflow.Server.DTOs;

public record RecurringExpenseResponse(
	int RecurringExpenseId,
	int FrequencyId,
	string FrequencyName,
	int ExpenseCategoryId,
	string CategoryName,
	string Description,
	DateTime StartDate,
	decimal Amount,
	bool AutomaticRun);

public record CreateRecurringExpenseRequest(
	int FrequencyId,
	int ExpenseCategoryId,
	string Description,
	DateTime StartDate,
	decimal Amount,
	bool AutomaticRun);

public record UpdateRecurringExpenseRequest(
	int FrequencyId,
	int ExpenseCategoryId,
	string Description,
	DateTime StartDate,
	decimal Amount,
	bool AutomaticRun);
