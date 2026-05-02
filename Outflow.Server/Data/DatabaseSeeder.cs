namespace Outflow.Server.Data;

public static class DatabaseSeeder
{
	public static async Task SeedAsync(AppDbContext db)
	{
		await db.SaveChangesAsync();
	}
}
