using System.ComponentModel.DataAnnotations;

namespace Outflow.Server.DTOs;

public record PayScheduleResponse(
	int PayScheduleId,
	int FrequencyId,
	string FrequencyName,
	decimal Amount,
	string Description,
	DateTime StartDate,
	DateTime? EndDate);

public record CreatePayScheduleRequest(
	int FrequencyId,
	decimal Amount,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	DateTime StartDate,
	DateTime? EndDate);

public record UpdatePayScheduleRequest(
	int FrequencyId,
	decimal Amount,
	[Required, StringLength(64, MinimumLength = 1)] string Description,
	DateTime StartDate,
	DateTime? EndDate);
