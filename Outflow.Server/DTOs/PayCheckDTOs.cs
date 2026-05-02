namespace Outflow.Server.DTOs;

public record PayCheckResponse(int PayCheckId, decimal Amount, DateTime Date);

public record CreatePayCheckRequest(decimal Amount, DateTime Date);

public record UpdatePayCheckRequest(decimal Amount, DateTime Date);
