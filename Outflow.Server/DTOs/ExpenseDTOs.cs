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
    string Description,
    DateTime Date,
    decimal Amount);

public record UpdateExpenseRequest(
    int ExpenseCategoryId,
    string Description,
    DateTime Date,
    decimal Amount);
