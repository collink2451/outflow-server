using System.ComponentModel.DataAnnotations;

namespace Outflow.Server.DTOs;

public record ExpenseResponse(
	int ExpenseId,
	int ExpenseCategoryId,
	string CategoryName,
	string Description,
	DateTime Date,
	decimal Amount);

public record CreateExpenseRequest(
	[Required] int ExpenseCategoryId,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	[Required] DateTime Date,
	[Required] decimal Amount);

public record UpdateExpenseRequest(
	[Required] int ExpenseCategoryId,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	[Required] DateTime Date,
	[Required] decimal Amount);
