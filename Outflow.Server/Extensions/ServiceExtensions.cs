using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using System.Threading.RateLimiting;

namespace Outflow.Server.Extensions;

public static class ServiceExtensions
{
	public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
	{
		string? connectionString = config.GetConnectionString("DefaultConnection");
		services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.Parse("8.0")));

		return services;
	}

	public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration config)
	{
		services.AddAuthentication(options =>
		{
			options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		})
		.AddCookie(options =>
		{
			options.Cookie.SameSite = SameSiteMode.None;
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			options.ExpireTimeSpan = TimeSpan.FromDays(30);
			options.SlidingExpiration = true;
			options.Events.OnRedirectToLogin = context =>
			{
				context.Response.StatusCode = 401;
				return Task.CompletedTask;
			};
			options.Events.OnSigningIn = context =>
			{
				context.Properties.IsPersistent = true;
				return Task.CompletedTask;
			};
		})
		.AddGoogle(options =>
		{
			options.ClientId = config["Authentication:Google:ClientId"]!;
			options.ClientSecret = config["Authentication:Google:ClientSecret"]!;
			options.CallbackPath = "/auth/google-callback";
			options.Scope.Add("email");
			options.Scope.Add("profile");
			options.CorrelationCookie.SameSite = SameSiteMode.None;
			options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
		});

		return services;
	}

	public static IServiceCollection AddAppCors(this IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy("AllowFrontend", policy =>
			{
				policy.WithOrigins(
						"http://localhost:4200",
						"https://outflow.collinkoldoff.dev")
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials();
			});
		});

		return services;
	}

	public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
	{
		services.AddRateLimiter(options =>
		{
			options.AddPolicy("api", httpContext =>
				RateLimitPartition.GetFixedWindowLimiter(
					partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
					factory: _ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = 30,
						Window = TimeSpan.FromMinutes(1),
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 0,
					}));

			options.RejectionStatusCode = 429;
		});

		return services;
	}

	public static IServiceCollection AddAppDataProtection(this IServiceCollection services)
	{
		services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo("/var/www/outflow/keys"));

		return services;
	}
}
