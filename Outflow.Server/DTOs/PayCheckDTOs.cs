using System.ComponentModel.DataAnnotations;

namespace Outflow.Server.DTOs;

public record PayCheckResponse(int PayCheckId, decimal Amount, DateTime Date);

public record CreatePayCheckRequest([Required] decimal Amount, [Required] DateTime Date);

public record UpdatePayCheckRequest([Required] decimal Amount, [Required] DateTime Date);
