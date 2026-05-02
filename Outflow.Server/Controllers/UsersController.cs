using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Outflow.Server.Data;
using Outflow.Server.DTOs;
using Outflow.Server.Models;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(AppDbContext db) : ApiControllerBase(db)
{
	[HttpGet("me")]
	public async Task<IActionResult> GetUser()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		return Ok(new UserResponse(user.UserId, user.Name, user.Email));
	}


	[HttpPut("me")]
	public async Task<IActionResult> PutUser([FromBody] UpdateUserRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		user.Name = request.Name;
		await Db.SaveChangesAsync();
		return Ok(new UserResponse(user.UserId, user.Name, user.Email));
	}


	[HttpDelete("me")]
	public async Task<IActionResult> DeleteMe()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();

		Db.Users.Remove(user);
		await Db.SaveChangesAsync();

		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return NoContent();
	}
}
