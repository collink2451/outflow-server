using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHostedService<DemoResetService>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
	options.Cookie.SameSite = SameSiteMode.None;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Events.OnRedirectToLogin = context =>
	{
		context.Response.StatusCode = 401;
		return Task.CompletedTask;
	};
})
.AddGoogle(options =>
{
	options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
	options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
	options.CallbackPath = "/auth/callback";
	options.Scope.Add("email");
	options.Scope.Add("profile");
	options.CorrelationCookie.SameSite = SameSiteMode.None;
	options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddRateLimiter(options =>
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

builder.Services.AddCors(options =>
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

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await DatabaseSeeder.SeedAsync(db);
await DemoDataSeeder.SeedAsync(db);

if (app.Environment.IsDevelopment())
	app.MapOpenApi();

// Cloudflare terminates SSL so Kestrel always sees http.
// Force https scheme so OAuth redirect URIs and cookies are built correctly.
app.Use(async (context, next) =>
{
	context.Request.Scheme = "https";
	await next();
});

app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("api");

app.Run();