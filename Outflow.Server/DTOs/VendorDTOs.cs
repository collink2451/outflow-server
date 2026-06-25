using System.ComponentModel.DataAnnotations;

namespace Outflow.Server.DTOs;

public record VendorResponse(
	int VendorId,
	string Name,
	string MatchPattern,
	int? ExpenseCategoryId,
	string? CategoryName,
	bool AutoDismiss);

public record CreateVendorRequest(
	[Required, StringLength(64, MinimumLength = 1)] string Name,
	[Required, StringLength(64, MinimumLength = 1)] string MatchPattern,
	int? ExpenseCategoryId,
	bool AutoDismiss);

public record UpdateVendorRequest(
	[Required, StringLength(64, MinimumLength = 1)] string Name,
	[Required, StringLength(64, MinimumLength = 1)] string MatchPattern,
	int? ExpenseCategoryId,
	bool AutoDismiss);
