using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.Models;
using System.Security.Claims;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
	private readonly string mFrontendUrl = config["FrontendUrl"]!;

	[HttpGet("login")]
	public IActionResult Login()
	{
		return Challenge(new AuthenticationProperties
		{
			RedirectUri = "/auth/callback"
		}, GoogleDefaults.AuthenticationScheme);
	}

	[HttpGet("callback")]
	public async Task<IActionResult> Callback()
	{
		AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		if (!result.Succeeded)
			return Unauthorized();

		IEnumerable<Claim> claims = result.Principal!.Claims;
		string? googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
		string? email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
		string? name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

		if (googleId == null)
			return Unauthorized();

		User? user = await db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
		if (user == null)
		{
			user = new User { GoogleId = googleId, Email = email!, Name = name! };
			db.Users.Add(user);
			await db.SaveChangesAsync();
		}

		return Redirect($"{mFrontendUrl}/dashboard");
	}

	[HttpGet("demo")]
	public async Task<IActionResult> DemoLogin()
	{
		User? user = await db.Users.FirstOrDefaultAsync(u => u.GoogleId == "demo");
		if (user == null) return NotFound();

		List<Claim> claims =
		[
			new(ClaimTypes.NameIdentifier, user.GoogleId),
			new(ClaimTypes.Email, user.Email),
			new(ClaimTypes.Name, user.Name)
		];

		ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		ClaimsPrincipal principal = new(identity);

		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

		return Redirect($"{mFrontendUrl}/dashboard");
	}

	[HttpGet("logout")]
	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return Redirect(mFrontendUrl);
	}

	[HttpGet("me")]
	[Authorize]
	public async Task<IActionResult> Me()
	{
		string? googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		User? user = await db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
		if (user == null) return Unauthorized();
		return Ok(new { user.UserId, user.Name, user.Email });
	}
}