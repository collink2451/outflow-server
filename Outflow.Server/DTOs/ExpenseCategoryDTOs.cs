using System.ComponentModel.DataAnnotations;

namespace Outflow.Server.DTOs;

public record ExpenseCategoryResponse(int ExpenseCategoryId, string Name);

public record CreateExpenseCategoryRequest([Required, StringLength(32, MinimumLength = 1)] string Name);

public record UpdateExpenseCategoryRequest([Required, StringLength(32, MinimumLength = 1)] string Name);
