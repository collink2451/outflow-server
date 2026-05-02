using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Outflow.Server.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var config = new ConfigurationBuilder()
			.AddUserSecrets<AppDbContextFactory>()
			.AddEnvironmentVariables()
			.Build();

		var connectionString = config.GetConnectionString("DefaultConnection")
			?? "server=localhost;database=outflow;user=root;password=placeholder";

		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)))
			.Options;

		return new AppDbContext(options);
	}
}
