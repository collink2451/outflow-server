namespace Outflow.Server.DTOs;

public record FrequencyResponse(int FrequencyId, string Name, int? DaysInterval, int? MonthsInterval);
