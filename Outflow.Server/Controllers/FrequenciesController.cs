using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.DTOs;

namespace Outflow.Server.Controllers;

[Route("api/[controller]")]
public class FrequenciesController(AppDbContext db) : ApiControllerBase(db)
{
	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		List<FrequencyResponse> frequencies = await Db.Frequencies
			.Select(f => new FrequencyResponse(f.FrequencyId, f.Name, f.DaysInterval, f.MonthsInterval))
			.ToListAsync();

		return Ok(frequencies);
	}
}
