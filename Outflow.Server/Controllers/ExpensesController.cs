using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.DTOs;
using Outflow.Server.Models;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController(AppDbContext db) : ApiControllerBase(db)
{
	[HttpGet]
	public async Task<IActionResult> GetExpenses()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		List<ExpenseResponse> expenses = await Db.Expenses
			.Where(e => e.UserId == user.UserId)
			.Select(e => new ExpenseResponse(e.ExpenseId, e.ExpenseCategoryId, e.ExpenseCategory.Name, e.Description, e.Date, e.Amount))
			.ToListAsync();

		return Ok(expenses);
	}

	[HttpGet("{expenseId}")]
	public async Task<IActionResult> GetExpense(int expenseId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		ExpenseResponse? expense = await Db.Expenses
			.Where(e => e.UserId == user.UserId && e.ExpenseId == expenseId)
			.Select(e => new ExpenseResponse(e.ExpenseId, e.ExpenseCategoryId, e.ExpenseCategory.Name, e.Description, e.Date, e.Amount))
			.FirstOrDefaultAsync();

		if (expense == null) return NotFound();

		return Ok(expense);
	}

	[HttpPost]
	public async Task<IActionResult> CreateExpense(CreateExpenseRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		Expense expense = new() { ExpenseCategoryId = request.ExpenseCategoryId, Description = request.Description, Date = request.Date, Amount = request.Amount, UserId = user.UserId };
		Db.Expenses.Add(expense);
		await Db.SaveChangesAsync();

		string categoryName = await Db.ExpenseCategories
			.Where(ec => ec.ExpenseCategoryId == expense.ExpenseCategoryId)
			.Select(ec => ec.Name)
			.FirstAsync();

		return Ok(new ExpenseResponse(expense.ExpenseId, expense.ExpenseCategoryId, categoryName, expense.Description, expense.Date, expense.Amount));
	}

	[HttpPut("{expenseId}")]
	public async Task<IActionResult> UpdateExpense(int expenseId, UpdateExpenseRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		Expense? expense = await Db.Expenses.FindAsync(expenseId);
		if (expense == null || expense.UserId != user.UserId) return NotFound();

		expense.ExpenseCategoryId = request.ExpenseCategoryId;
		expense.Description = request.Description;
		expense.Date = request.Date;
		expense.Amount = request.Amount;

		await Db.SaveChangesAsync();

		string categoryName = await Db.ExpenseCategories
			.Where(ec => ec.ExpenseCategoryId == expense.ExpenseCategoryId)
			.Select(ec => ec.Name)
			.FirstAsync();

		return Ok(new ExpenseResponse(expense.ExpenseId, expense.ExpenseCategoryId, categoryName, expense.Description, expense.Date, expense.Amount));
	}

	[HttpDelete("{expenseId}")]
	public async Task<IActionResult> DeleteExpense(int expenseId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		Expense? expense = await Db.Expenses.FindAsync(expenseId);
		if (expense == null || expense.UserId != user.UserId) return NotFound();

		Db.Expenses.Remove(expense);
		await Db.SaveChangesAsync();

		return NoContent();
	}
}
