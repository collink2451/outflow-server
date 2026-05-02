using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.DTOs;
using Outflow.Server.Models;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaySchedulesController(AppDbContext db) : ApiControllerBase(db)
{
	[HttpGet]
	public async Task<IActionResult> GetPaySchedules()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		List<PayScheduleResponse> paySchedules = await Db.PaySchedules
			.Where(e => e.UserId == user.UserId)
			.Select(e => new PayScheduleResponse(e.PayScheduleId, e.FrequencyId, e.Frequency.Name, e.Amount, e.Description, e.StartDate, e.EndDate))
			.ToListAsync();

		return Ok(paySchedules);
	}

	[HttpGet("{payScheduleId}")]
	public async Task<IActionResult> GetPaySchedule(int payScheduleId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		PayScheduleResponse? paySchedule = await Db.PaySchedules
			.Where(ps => ps.UserId == user.UserId && ps.PayScheduleId == payScheduleId)
			.Select(ps => new PayScheduleResponse(ps.PayScheduleId, ps.FrequencyId, ps.Frequency.Name, ps.Amount, ps.Description, ps.StartDate, ps.EndDate))
			.FirstOrDefaultAsync();

		if (paySchedule == null) return NotFound();

		return Ok(paySchedule);
	}

	[HttpPost]
	public async Task<IActionResult> CreatePaySchedule(CreatePayScheduleRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		PaySchedule paySchedule = new() { FrequencyId = request.FrequencyId, Amount = request.Amount, Description = request.Description, StartDate = request.StartDate, EndDate = request.EndDate, UserId = user.UserId };
		Db.PaySchedules.Add(paySchedule);
		await Db.SaveChangesAsync();

		string frequencyName = await Db.Frequencies
			.Where(f => f.FrequencyId == paySchedule.FrequencyId)
			.Select(f => f.Name)
			.FirstAsync();

		return Ok(new PayScheduleResponse(paySchedule.PayScheduleId, paySchedule.FrequencyId, frequencyName, paySchedule.Amount, paySchedule.Description, paySchedule.StartDate, paySchedule.EndDate));
	}

	[HttpPut("{payScheduleId}")]
	public async Task<IActionResult> UpdatePaySchedule(int payScheduleId, UpdatePayScheduleRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		PaySchedule? paySchedule = await Db.PaySchedules.FindAsync(payScheduleId);
		if (paySchedule == null || paySchedule.UserId != user.UserId) return NotFound();

		paySchedule.FrequencyId = request.FrequencyId;
		paySchedule.Amount = request.Amount;
		paySchedule.Description = request.Description;
		paySchedule.StartDate = request.StartDate;
		paySchedule.EndDate = request.EndDate;

		await Db.SaveChangesAsync();

		string frequencyName = await Db.Frequencies
			.Where(f => f.FrequencyId == paySchedule.FrequencyId)
			.Select(f => f.Name)
			.FirstAsync();

		return Ok(new PayScheduleResponse(paySchedule.PayScheduleId, paySchedule.FrequencyId, frequencyName, paySchedule.Amount, paySchedule.Description, paySchedule.StartDate, paySchedule.EndDate));
	}

	[HttpDelete("{payScheduleId}")]
	public async Task<IActionResult> DeletePaySchedule(int payScheduleId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		PaySchedule? paySchedule = await Db.PaySchedules.FindAsync(payScheduleId);
		if (paySchedule == null || paySchedule.UserId != user.UserId) return NotFound();

		Db.PaySchedules.Remove(paySchedule);
		await Db.SaveChangesAsync();

		return NoContent();
	}
}
