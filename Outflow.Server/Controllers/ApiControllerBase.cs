using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.Models;
using System.Security.Claims;

namespace Outflow.Server.Controllers;

[Authorize]
[ApiController]
public abstract class ApiControllerBase(AppDbContext db) : ControllerBase
{
	protected readonly AppDbContext Db = db;
	protected async Task<User?> GetCurrentUserAsync()
	{
		string? googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (googleId == null) return null;
		return await Db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
	}
}