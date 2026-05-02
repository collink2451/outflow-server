using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.DTOs;
using Outflow.Server.Models;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayChecksController(AppDbContext db) : ApiControllerBase(db)
{
	[HttpGet]
	public async Task<IActionResult> GetPayChecks()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		List<PayCheckResponse> payChecks = await Db.PayChecks
			.Where(e => e.UserId == user.UserId)
			.Select(e => new PayCheckResponse(e.PayCheckId, e.Amount, e.Date))
			.ToListAsync();

		return Ok(payChecks);
	}

	[HttpGet("{payCheckId}")]
	public async Task<IActionResult> GetPayCheck(int payCheckId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		PayCheckResponse? payCheck = await Db.PayChecks
			.Where(pc => pc.UserId == user.UserId && pc.PayCheckId == payCheckId)
			.Select(pc => new PayCheckResponse(pc.PayCheckId, pc.Amount, pc.Date))
			.FirstOrDefaultAsync();

		if (payCheck == null) return NotFound();

		return Ok(payCheck);
	}

	[HttpPost]
	public async Task<IActionResult> CreatePayCheck(CreatePayCheckRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		PayCheck payCheck = new() { Amount = request.Amount, Date = request.Date, UserId = user.UserId };
		Db.PayChecks.Add(payCheck);
		await Db.SaveChangesAsync();

		return Ok(new PayCheckResponse(payCheck.PayCheckId, payCheck.Amount, payCheck.Date));
	}

	[HttpPut("{payCheckId}")]
	public async Task<IActionResult> UpdatePayCheck(int payCheckId, UpdatePayCheckRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		PayCheck? payCheck = await Db.PayChecks.FindAsync(payCheckId);
		if (payCheck == null || payCheck.UserId != user.UserId) return NotFound();

		payCheck.Amount = request.Amount;
		payCheck.Date = request.Date;

		await Db.SaveChangesAsync();

		return Ok(new PayCheckResponse(payCheck.PayCheckId, payCheck.Amount, payCheck.Date));
	}

	[HttpDelete("{payCheckId}")]
	public async Task<IActionResult> DeletePayCheck(int payCheckId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		PayCheck? payCheck = await Db.PayChecks.FindAsync(payCheckId);
		if (payCheck == null || payCheck.UserId != user.UserId) return NotFound();

		Db.PayChecks.Remove(payCheck);
		await Db.SaveChangesAsync();

		return NoContent();
	}
}
