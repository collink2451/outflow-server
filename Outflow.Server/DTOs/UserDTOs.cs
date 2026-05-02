namespace Outflow.Server.DTOs;

public record UserResponse(int UserId, string Name, string Email);

public record UpdateUserRequest(string Name);
