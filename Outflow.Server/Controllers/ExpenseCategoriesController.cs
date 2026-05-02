using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.DTOs;
using Outflow.Server.Models;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpenseCategoriesController(AppDbContext db) : ApiControllerBase(db)
{
	[HttpGet]
	public async Task<IActionResult> GetExpenseCategories()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		List<ExpenseCategoryResponse> categories = await Db.ExpenseCategories
			.Where(ec => ec.UserId == user.UserId)
			.Select(ec => new ExpenseCategoryResponse(ec.ExpenseCategoryId, ec.Name))
			.ToListAsync();

		return Ok(categories);
	}

	[HttpPost]
	public async Task<IActionResult> CreateExpenseCategory(CreateExpenseCategoryRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		ExpenseCategory category = new() { Name = request.Name, UserId = user.UserId };
		Db.ExpenseCategories.Add(category);
		await Db.SaveChangesAsync();

		return Ok(new ExpenseCategoryResponse(category.ExpenseCategoryId, category.Name));
	}

	[HttpPut("{expenseCategoryId}")]
	public async Task<IActionResult> UpdateExpenseCategory(int expenseCategoryId, UpdateExpenseCategoryRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		ExpenseCategory? category = await Db.ExpenseCategories.FindAsync(expenseCategoryId);
		if (category == null || category.UserId != user.UserId) return NotFound();

		category.Name = request.Name;
		await Db.SaveChangesAsync();

		return Ok(new ExpenseCategoryResponse(category.ExpenseCategoryId, category.Name));
	}

	[HttpDelete("{expenseCategoryId}")]
	public async Task<IActionResult> DeleteExpenseCategory(int expenseCategoryId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		ExpenseCategory? category = await Db.ExpenseCategories.FindAsync(expenseCategoryId);
		if (category == null || category.UserId != user.UserId) return NotFound();

		Db.ExpenseCategories.Remove(category);
		await Db.SaveChangesAsync();

		return NoContent();
	}
}
