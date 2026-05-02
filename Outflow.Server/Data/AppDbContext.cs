using Microsoft.EntityFrameworkCore;

namespace Outflow.Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

	}
}
