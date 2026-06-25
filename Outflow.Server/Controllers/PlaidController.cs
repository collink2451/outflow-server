using Going.Plaid;
using Going.Plaid.Entity;
using Going.Plaid.Item;
using Going.Plaid.Link;
using Going.Plaid.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.DTOs;
using Outflow.Server.Models;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaidController(AppDbContext db, PlaidClient _plaidClient) : ApiControllerBase(db)
{
	[HttpGet("connections")]
	public async Task<IActionResult> GetConnections()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		List<PlaidConnectionResponse> connections = await Db.PlaidConnections
			.Where(c => c.UserId == user.UserId)
			.Select(c => new PlaidConnectionResponse(c.PlaidConnectionId, c.InstitutionName, c.InstitutionId))
			.ToListAsync();

		return Ok(connections);
	}

	[HttpPost("link-token")]
	public async Task<IActionResult> PostLinkToken()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		LinkTokenCreateResponse response = await _plaidClient.LinkTokenCreateAsync(new LinkTokenCreateRequest
		{
			User = new LinkTokenCreateRequestUser { ClientUserId = user.UserId.ToString() },
			ClientName = "Outflow",
			Products = [Products.Transactions],
			CountryCodes = [CountryCode.Us],
			Language = Language.English
		});

		return Ok(new { response.LinkToken });
	}

	[HttpPost("exchange-token")]
	public async Task<IActionResult> PostExchangeToken(ExchangeTokenRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		ItemPublicTokenExchangeResponse response = await _plaidClient.ItemPublicTokenExchangeAsync(new ItemPublicTokenExchangeRequest
		{
			PublicToken = request.PublicToken
		});

		Db.PlaidConnections.Add(new PlaidConnection
		{
			UserId = user.UserId,
			AccessToken = response.AccessToken,
			ItemId = response.ItemId,
			InstitutionId = request.InstitutionId,
			InstitutionName = request.InstitutionName
		});

		await Db.SaveChangesAsync();

		return Ok();
	}

	[HttpPost("sync")]
	public async Task<IActionResult> PostSync()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		List<PlaidConnection> connections = await Db.PlaidConnections
			.Where(c => c.UserId == user.UserId)
			.ToListAsync();

		foreach (PlaidConnection connection in connections)
		{
			bool hasMore = true;
			while (hasMore)
			{
				TransactionsSyncResponse response = await _plaidClient.TransactionsSyncAsync(new TransactionsSyncRequest
				{
					AccessToken = connection.AccessToken,
					Cursor = connection.Cursor
				});

				// TODO handle responses here

				connection.Cursor = response.NextCursor;
				hasMore = response.HasMore;
			}
		}

		return Ok();
	}

	[HttpDelete("connections/{connectionId}")]
	public async Task<IActionResult> DeleteConnection(int connectionId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		PlaidConnection? connection = await Db.PlaidConnections.FindAsync(connectionId);
		if (connection == null || connection.UserId != user.UserId) return NotFound();

		await _plaidClient.ItemRemoveAsync(new ItemRemoveRequest
		{
			AccessToken = connection.AccessToken
		});

		Db.PlaidConnections.Remove(connection);
		await Db.SaveChangesAsync();

		return NoContent();
	}
}
