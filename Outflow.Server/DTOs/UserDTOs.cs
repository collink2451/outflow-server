using System.ComponentModel.DataAnnotations;

namespace Outflow.Server.DTOs;

public record UserResponse(int UserId, string Name, string Email);

public record UpdateUserRequest([Required, StringLength(64, MinimumLength = 1)] string Name);
