using System.ComponentModel.DataAnnotations;

namespace Outflow.Server.Models;

public class ServiceTimestamp
{
    [Key]
    public required string ServiceName { get; set; }
    public DateTime LastRunAt { get; set; }
}
