using Outflow.Server.Data;
using Outflow.Server.Extensions;
using Outflow.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHostedService<DemoResetService>();
builder.Services.AddHostedService<RecurringExpenseService>();
builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddAppRateLimiting();
builder.Services.AddAppCors();

if (builder.Environment.IsProduction()) builder.Services.AddAppDataProtection();

var app = builder.Build();

// Seed the database with demo data on startup
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await DemoDataSeeder.SeedAsync(db);


if (app.Environment.IsDevelopment())
	app.MapOpenApi();

app.UseAppExceptionHandling();
app.UseReverseProxyHeaders();
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("api");

app.Run();