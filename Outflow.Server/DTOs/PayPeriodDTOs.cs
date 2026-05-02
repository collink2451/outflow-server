namespace Outflow.Server.DTOs;

public record PayPeriodResponse(
	PayCheckResponse PayCheck,
	List<ExpenseResponse> Expenses,
	decimal TotalExpenses,
	decimal Net);
