using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outflow.Server.Models;

public class PlaidConnection : BaseEntity
{
	public int PlaidConnectionId { get; set; }
	public int UserId { get; set; }
	public User User { get; set; } = null!;
	public string AccessToken { get; set; } = "";
	public string ItemId { get; set; } = "";
	public string InstitutionId { get; set; } = "";
	public string InstitutionName { get; set; } = "";
	public string? Cursor { get; set; }
}

public class PlaidConnectionConfiguration : IEntityTypeConfiguration<PlaidConnection>
{
	public void Configure(EntityTypeBuilder<PlaidConnection> builder)
	{
		builder.HasOne(pc => pc.User)
			.WithMany()
			.HasForeignKey(pc => pc.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
