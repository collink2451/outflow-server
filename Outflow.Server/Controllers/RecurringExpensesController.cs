using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.DTOs;
using Outflow.Server.Models;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecurringExpensesController(AppDbContext db) : ApiControllerBase(db)
{
	[HttpGet]
	public async Task<IActionResult> GetRecurringExpenses()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		List<RecurringExpenseResponse> recurringExpenses = await Db.RecurringExpenses
			.Where(e => e.UserId == user.UserId)
			.Select(e => new RecurringExpenseResponse(
				e.RecurringExpenseId,
				e.FrequencyId,
				e.Frequency.Name,
				e.ExpenseCategoryId,
				e.ExpenseCategory.Name,
				e.Description,
				e.StartDate,
				e.Amount,
				e.AutomaticRun
			))
			.ToListAsync();

		return Ok(recurringExpenses);
	}

	[HttpGet("{recurringExpenseId}")]
	public async Task<IActionResult> GetRecurringExpense(int recurringExpenseId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		RecurringExpenseResponse? recurringExpense = await Db.RecurringExpenses
			.Where(re => re.UserId == user.UserId && re.RecurringExpenseId == recurringExpenseId)
			.Select(re => new RecurringExpenseResponse(
				re.RecurringExpenseId,
				re.FrequencyId,
				re.Frequency.Name,
				re.ExpenseCategoryId,
				re.ExpenseCategory.Name,
				re.Description,
				re.StartDate,
				re.Amount,
				re.AutomaticRun
			))
			.FirstOrDefaultAsync();

		if (recurringExpense == null) return NotFound();

		return Ok(recurringExpense);
	}

	[HttpPost]
	public async Task<IActionResult> CreateRecurringExpense(CreateRecurringExpenseRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		RecurringExpense recurringExpense = new()
		{
			FrequencyId = request.FrequencyId,
			ExpenseCategoryId = request.ExpenseCategoryId,
			Description = request.Description,
			StartDate = request.StartDate,
			Amount = request.Amount,
			UserId = user.UserId,
			AutomaticRun = request.AutomaticRun
		};
		Db.RecurringExpenses.Add(recurringExpense);
		await Db.SaveChangesAsync();

		string categoryName = await Db.ExpenseCategories
			.Where(ec => ec.ExpenseCategoryId == recurringExpense.ExpenseCategoryId)
			.Select(ec => ec.Name)
			.FirstAsync();

		string frequencyName = await Db.Frequencies
			.Where(f => f.FrequencyId == recurringExpense.FrequencyId)
			.Select(f => f.Name)
			.FirstAsync();

		return Ok(new RecurringExpenseResponse(
			recurringExpense.RecurringExpenseId,
			recurringExpense.FrequencyId,
			frequencyName,
			recurringExpense.ExpenseCategoryId,
			categoryName,
			recurringExpense.Description,
			recurringExpense.StartDate,
			recurringExpense.Amount,
			recurringExpense.AutomaticRun
		));
	}

	[HttpPut("{recurringExpenseId}")]
	public async Task<IActionResult> UpdateRecurringExpense(int recurringExpenseId, UpdateRecurringExpenseRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		RecurringExpense? recurringExpense = await Db.RecurringExpenses.FindAsync(recurringExpenseId);
		if (recurringExpense == null || recurringExpense.UserId != user.UserId) return NotFound();

		recurringExpense.FrequencyId = request.FrequencyId;
		recurringExpense.ExpenseCategoryId = request.ExpenseCategoryId;
		recurringExpense.Description = request.Description;
		recurringExpense.StartDate = request.StartDate;
		recurringExpense.Amount = request.Amount;
		recurringExpense.AutomaticRun = request.AutomaticRun;

		await Db.SaveChangesAsync();

		string categoryName = await Db.ExpenseCategories
			.Where(ec => ec.ExpenseCategoryId == recurringExpense.ExpenseCategoryId)
			.Select(ec => ec.Name)
			.FirstAsync();

		string frequencyName = await Db.Frequencies
			.Where(f => f.FrequencyId == recurringExpense.FrequencyId)
			.Select(f => f.Name)
			.FirstAsync();

		return Ok(new RecurringExpenseResponse(
			recurringExpense.RecurringExpenseId,
			recurringExpense.FrequencyId,
			frequencyName,
			recurringExpense.ExpenseCategoryId,
			categoryName,
			recurringExpense.Description,
			recurringExpense.StartDate,
			recurringExpense.Amount,
			recurringExpense.AutomaticRun
		));
	}

	[HttpDelete("{recurringExpenseId}")]
	public async Task<IActionResult> DeleteRecurringExpense(int recurringExpenseId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		RecurringExpense? recurringExpense = await Db.RecurringExpenses.FindAsync(recurringExpenseId);
		if (recurringExpense == null || recurringExpense.UserId != user.UserId) return NotFound();

		Db.RecurringExpenses.Remove(recurringExpense);
		await Db.SaveChangesAsync();

		return NoContent();
	}
}
