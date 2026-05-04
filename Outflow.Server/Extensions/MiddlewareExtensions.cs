using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;

namespace Outflow.Server.Extensions;

public static class MiddlewareExtensions
{
	public static WebApplication UseAppExceptionHandling(this WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
			app.UseExceptionHandler(appBuilder =>
				appBuilder.Run(async context =>
				{
					var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
					var feature = context.Features.Get<IExceptionHandlerFeature>();
					if (feature?.Error is { } ex)
						logger.LogError(ex, "Unhandled exception at {Path}", context.Request.Path);

					context.Response.StatusCode = 500;
					context.Response.ContentType = "application/json";
					await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
				}));

		return app;
	}

	public static WebApplication UseReverseProxyHeaders(this WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
			app.Use(async (context, next) =>
			{
				context.Request.Scheme = "https";
				await next();
			});

		app.UseForwardedHeaders(new ForwardedHeadersOptions
		{
			ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
		});

		return app;
	}
}
