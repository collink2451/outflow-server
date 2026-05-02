namespace Outflow.Server.DTOs;

public record ExpenseCategoryResponse(int ExpenseCategoryId, string Name);

public record CreateExpenseCategoryRequest(string Name);

public record UpdateExpenseCategoryRequest(string Name);
