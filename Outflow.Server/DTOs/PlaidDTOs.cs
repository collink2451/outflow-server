using System.ComponentModel.DataAnnotations;

namespace Outflow.Server.DTOs;

public record ExchangeTokenRequest(
	[Required] string PublicToken,
	[Required] string InstitutionId,
	[Required] string InstitutionName);

public record PlaidConnectionResponse(
	int PlaidConnectionId,
	string InstitutionName,
	string InstitutionId);
