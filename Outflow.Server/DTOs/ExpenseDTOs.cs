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
	int ExpenseCategoryId,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	DateTime Date,
	decimal Amount);

public record UpdateExpenseRequest(
	int ExpenseCategoryId,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	DateTime Date,
	decimal Amount);
