using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class User : BaseEntity
{
	public int UserId { get; set; }
	public string GoogleId { get; set; } = "";
	public string Email { get; set; } = "";
	public string Name { get; set; } = "";
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasIndex(u => u.GoogleId).IsUnique();
	}
}
