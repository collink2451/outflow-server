using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outflow.Server.Data;
using Outflow.Server.DTOs;
using Outflow.Server.Models;

namespace Outflow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorsController(AppDbContext db) : ApiControllerBase(db)
{
	[HttpGet]
	public async Task<IActionResult> GetVendors()
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		List<VendorResponse> vendors = await Db.Vendors
			.Where(v => v.UserId == user.UserId)
			.Select(v => new VendorResponse(v.VendorId, v.Name, v.MatchPattern, v.ExpenseCategoryId, v.ExpenseCategory != null ? v.ExpenseCategory.Name : null, v.AutoDismiss))
			.ToListAsync();

		return Ok(vendors);
	}

	[HttpPost]
	public async Task<IActionResult> CreateVendor(CreateVendorRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		Vendor vendor = new()
		{
			UserId = user.UserId,
			Name = request.Name,
			MatchPattern = request.MatchPattern,
			ExpenseCategoryId = request.ExpenseCategoryId,
			AutoDismiss = request.AutoDismiss
		};

		Db.Vendors.Add(vendor);
		await Db.SaveChangesAsync();

		string? categoryName = vendor.ExpenseCategoryId == null ? null : await Db.ExpenseCategories
			.Where(ec => ec.ExpenseCategoryId == vendor.ExpenseCategoryId)
			.Select(ec => ec.Name)
			.FirstAsync();

		return Ok(new VendorResponse(vendor.VendorId, vendor.Name, vendor.MatchPattern, vendor.ExpenseCategoryId, categoryName, vendor.AutoDismiss));
	}

	[HttpPut("{vendorId}")]
	public async Task<IActionResult> UpdateVendor(int vendorId, UpdateVendorRequest request)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		Vendor? vendor = await Db.Vendors.FindAsync(vendorId);
		if (vendor == null || vendor.UserId != user.UserId) return NotFound();

		vendor.Name = request.Name;
		vendor.MatchPattern = request.MatchPattern;
		vendor.ExpenseCategoryId = request.ExpenseCategoryId;
		vendor.AutoDismiss = request.AutoDismiss;

		await Db.SaveChangesAsync();

		string? categoryName = vendor.ExpenseCategoryId == null ? null : await Db.ExpenseCategories
			.Where(ec => ec.ExpenseCategoryId == vendor.ExpenseCategoryId)
			.Select(ec => ec.Name)
			.FirstAsync();

		return Ok(new VendorResponse(vendor.VendorId, vendor.Name, vendor.MatchPattern, vendor.ExpenseCategoryId, categoryName, vendor.AutoDismiss));
	}

	[HttpDelete("{vendorId}")]
	public async Task<IActionResult> DeleteVendor(int vendorId)
	{
		User? user = await GetCurrentUserAsync();
		if (user == null) return Unauthorized();
		if (user.GoogleId == "demo") return Forbid();

		Vendor? vendor = await Db.Vendors.FindAsync(vendorId);
		if (vendor == null || vendor.UserId != user.UserId) return NotFound();

		Db.Vendors.Remove(vendor);
		await Db.SaveChangesAsync();

		return NoContent();
	}
}
