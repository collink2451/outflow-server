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

		List<Vendor> vendors = await Db.Vendors
			.Where(v => v.UserId == user.UserId)
			.ToListAsync();

		HashSet<string> stagedIds = [.. (await Db.PlaidTransactions
			.Where(pt => pt.UserId == user.UserId)
			.Select(pt => pt.PlaidTransactionId)
			.ToListAsync())];

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

				foreach (Transaction added in response.Added)
				{
					if (added.TransactionId == null || stagedIds.Contains(added.TransactionId)) continue;

					stagedIds.Add(added.TransactionId);

					string merchantName = added.MerchantName ?? "";

					Vendor? vendor = vendors.FirstOrDefault(v =>
						merchantName.Contains(v.MatchPattern, StringComparison.OrdinalIgnoreCase));

					if (vendor != null && vendor.AutoDismiss)
					{
						// intentionally skipped
					}
					else if (vendor != null && vendor.ExpenseCategoryId != null)
					{
						Db.Expenses.Add(new Expense
						{
							UserId = user.UserId,
							ExpenseCategoryId = vendor.ExpenseCategoryId.Value,
							Description = vendor.Name,
							Date = added.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow,
							Amount = added.Amount ?? 0
						});
					}
					else
					{
						Db.PlaidTransactions.Add(new PlaidTransaction
						{
							PlaidTransactionId = added.TransactionId,
							UserId = user.UserId,
							PlaidConnectionId = connection.PlaidConnectionId,
							Name = merchantName,
							Date = added.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow,
							Amount = added.Amount ?? 0
						});
					}
				}

				foreach (Transaction modified in response.Modified)
				{
					PlaidTransaction? staged = await Db.PlaidTransactions.FindAsync(modified.TransactionId);
					if (staged == null) continue;

					staged.Name = modified.MerchantName ?? staged.Name;
					staged.Date = modified.Date?.ToDateTime(TimeOnly.MinValue) ?? staged.Date;
					staged.Amount = modified.Amount ?? staged.Amount;
				}

				foreach (RemovedTransaction removed in response.Removed)
				{
					PlaidTransaction? staged = await Db.PlaidTransactions.FindAsync(removed.TransactionId);
					if (staged != null) Db.PlaidTransactions.Remove(staged);
				}

				connection.Cursor = response.NextCursor;
				hasMore = response.HasMore;
			}
		}

		await Db.SaveChangesAsync();
		return Ok();
	}

	[HttpGet("staged")]
	public async Task<IActionResult> GetStaged()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		List<PlaidTransactionResponse> transactions = await Db.PlaidTransactions
			.Where(pt => pt.UserId == user.UserId)
			.OrderBy(pt => pt.Date)
			.Select(pt => new PlaidTransactionResponse(
				pt.PlaidTransactionId,
				pt.PlaidConnectionId,
				pt.PlaidConnection.InstitutionName,
				pt.Name,
				pt.Date,
				pt.Amount))
			.ToListAsync();

		return Ok(transactions);
	}

	[HttpPost("staged/{plaidTransactionId}/approve")]
	public async Task<IActionResult> PostApproveTransaction(string plaidTransactionId, ApproveTransactionRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		PlaidTransaction? transaction = await Db.PlaidTransactions.FindAsync(plaidTransactionId);
		if (transaction == null || transaction.UserId != user.UserId) return NotFound();

		Db.Expenses.Add(new Expense
		{
			UserId = user.UserId,
			ExpenseCategoryId = request.ExpenseCategoryId,
			Description = request.Description ?? transaction.Name,
			Date = transaction.Date,
			Amount = transaction.Amount
		});

		Db.PlaidTransactions.Remove(transaction);
		await Db.SaveChangesAsync();

		return Ok();
	}

	[HttpDelete("staged/{plaidTransactionId}")]
	public async Task<IActionResult> DeleteStagedTransaction(string plaidTransactionId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		PlaidTransaction? transaction = await Db.PlaidTransactions.FindAsync(plaidTransactionId);
		if (transaction == null || transaction.UserId != user.UserId) return NotFound();

		Db.PlaidTransactions.Remove(transaction);
		await Db.SaveChangesAsync();

		return NoContent();
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
