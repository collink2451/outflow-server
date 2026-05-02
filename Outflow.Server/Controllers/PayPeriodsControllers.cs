using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.DTOs;
using Outflow.Server.Models;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayPeriodsController(AppDbContext db) : ApiControllerBase(db)
{
	[HttpGet]
	public async Task<IActionResult> GetPayPeriods()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		List<PayCheck> payChecks = await Db.PayChecks
			.Where(pc => pc.UserId == user.UserId)
			.OrderBy(pc => pc.Date)
			.ToListAsync();

		List<Expense> expenses = await Db.Expenses
			.Where(e => e.UserId == user.UserId)
			.Include(e => e.ExpenseCategory)
			.ToListAsync();

		List<PayPeriodResponse> payPeriods = [.. payChecks.Select(pc =>
		{
			List<Expense> periodExpenses = [.. expenses
				.Where(e => e.Date >= pc.Date)
				.Where(e => payChecks
					.Where(next => next.Date > pc.Date)
					.All(next => e.Date < next.Date)
				)];

			decimal totalSpent = periodExpenses.Sum(e => e.Amount);

			return new PayPeriodResponse(
				new PayCheckResponse(pc.PayCheckId, pc.Amount, pc.Date),
				[.. periodExpenses.Select(e => new ExpenseResponse(e.ExpenseId, e.ExpenseCategoryId, e.ExpenseCategory.Name, e.Description, e.Date, e.Amount))],
				totalSpent,
				pc.Amount - totalSpent
			);
		})];


		return Ok(payPeriods);
	}
}
