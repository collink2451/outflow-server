using Outflow.Server.Models;

namespace Outflow.Server.Data;

public static class DemoDataSeeder
{
	public static async Task SeedAsync(AppDbContext db)
	{
		const string DEMO_GOOGLE_ID = "demo";

		User? existingUser = db.Users.FirstOrDefault(u => u.GoogleId == DEMO_GOOGLE_ID);
		if (existingUser != null)
			await ClearDemoDataAsync(db, existingUser.UserId);

		// All dates are computed as offsets from today.
		// Day 0 = today (last expense), day -118 = ~4 months ago (first expense/paycheck).
		DateTime today = DateTime.UtcNow.Date;

		// -------------------------
		// User
		// -------------------------
		User user = new()
		{
			GoogleId = DEMO_GOOGLE_ID,
			Email = "demo@outflow.app",
			Name = "Demo User"
		};
		db.Users.Add(user);
		await db.SaveChangesAsync();

		int userId = user.UserId;

		// -------------------------
		// Expense Categories
		// -------------------------
		ExpenseCategory housing = new() { UserId = userId, Name = "Housing" };
		ExpenseCategory groceries = new() { UserId = userId, Name = "Groceries" };
		ExpenseCategory diningOut = new() { UserId = userId, Name = "Dining Out" };
		ExpenseCategory transport = new() { UserId = userId, Name = "Transport" };
		ExpenseCategory utilities = new() { UserId = userId, Name = "Utilities" };
		ExpenseCategory entertainment = new() { UserId = userId, Name = "Entertainment" };
		ExpenseCategory health = new() { UserId = userId, Name = "Health" };
		ExpenseCategory shopping = new() { UserId = userId, Name = "Shopping" };

		db.ExpenseCategories.AddRange(housing, groceries, diningOut, transport, utilities, entertainment, health, shopping);
		await db.SaveChangesAsync();

		// -------------------------
		// Frequencies
		// -------------------------
		Frequency biWeekly = db.Frequencies.First(f => f.Name == "BiWeekly");
		Frequency monthly = db.Frequencies.First(f => f.Name == "Monthly");
		Frequency biYearly = db.Frequencies.First(f => f.Name == "BiYearly");
		Frequency yearly = db.Frequencies.First(f => f.Name == "Yearly");

		// -------------------------
		// Pay Schedule
		// -------------------------
		PaySchedule paySchedule = new()
		{
			UserId = userId,
			FrequencyId = biWeekly.FrequencyId,
			Amount = 4200.00m,
			StartDate = DateOffset(today, -118),
			EndDate = null
		};
		db.PaySchedules.Add(paySchedule);
		await db.SaveChangesAsync();

		// -------------------------
		// PayChecks
		// Biweekly starting day -118, every 14 days
		// -------------------------
		int[] paycheckOffsets = [-118, -104, -90, -76, -62, -48, -34, -20, -6];

		db.PayChecks.AddRange(paycheckOffsets.Select(offset => new PayCheck
		{
			UserId = userId,
			Amount = 4200.00m,
			Date = DateOffset(today, offset)
		}));
		await db.SaveChangesAsync();

		// -------------------------
		// Recurring Expenses
		// -------------------------
		DateTime recurringStart = DateOffset(today, -118);

		db.RecurringExpenses.AddRange(
			new RecurringExpense
			{
				UserId = userId,
				FrequencyId = monthly.FrequencyId,
				ExpenseCategoryId = housing.ExpenseCategoryId,
				Description = "Rent",
				Amount = 1650.00m,
				StartDate = recurringStart
			},
			new RecurringExpense
			{
				UserId = userId,
				FrequencyId = monthly.FrequencyId,
				ExpenseCategoryId = housing.ExpenseCategoryId,
				Description = "Renters Insurance",
				Amount = 18.00m,
				StartDate = recurringStart
			},
			new RecurringExpense
			{
				UserId = userId,
				FrequencyId = monthly.FrequencyId,
				ExpenseCategoryId = utilities.ExpenseCategoryId,
				Description = "Electric Bill",
				Amount = 85.00m,
				StartDate = recurringStart
			},
			new RecurringExpense
			{
				UserId = userId,
				FrequencyId = monthly.FrequencyId,
				ExpenseCategoryId = utilities.ExpenseCategoryId,
				Description = "Internet",
				Amount = 59.99m,
				StartDate = recurringStart
			},
			new RecurringExpense
			{
				UserId = userId,
				FrequencyId = monthly.FrequencyId,
				ExpenseCategoryId = entertainment.ExpenseCategoryId,
				Description = "Spotify",
				Amount = 11.99m,
				StartDate = recurringStart
			},
			new RecurringExpense
			{
				UserId = userId,
				FrequencyId = monthly.FrequencyId,
				ExpenseCategoryId = entertainment.ExpenseCategoryId,
				Description = "Netflix",
				Amount = 15.49m,
				StartDate = recurringStart
			},
			new RecurringExpense
			{
				UserId = userId,
				FrequencyId = monthly.FrequencyId,
				ExpenseCategoryId = health.ExpenseCategoryId,
				Description = "Gym",
				Amount = 35.00m,
				StartDate = recurringStart
			},
			new RecurringExpense
			{
				UserId = userId,
				FrequencyId = biYearly.FrequencyId,
				ExpenseCategoryId = transport.ExpenseCategoryId,
				Description = "Car Insurance",
				Amount = 700.00m,
				StartDate = recurringStart
			},
			new RecurringExpense
			{
				UserId = userId,
				FrequencyId = yearly.FrequencyId,
				ExpenseCategoryId = shopping.ExpenseCategoryId,
				Description = "Amazon Prime",
				Amount = 139.00m,
				StartDate = recurringStart
			}
		);
		await db.SaveChangesAsync();

		// -------------------------
		// Expenses
		// Offsets from today. Day 0 = today (last expense), day -118 = first expense.
		// Pay periods (biweekly): -118..-105, -104..-91, -90..-77, -76..-63, -62..-49, -48..-35, -34..-21, -20..-7, -6..0
		//
		// Car Insurance (BiYearly = every 6 months): one occurrence at day -90 within this window.
		// Amazon Prime (Yearly): one occurrence at day -118 (start of window).
		// -------------------------
		List<Expense> expenses =
		[
			// Period 1: day -118 to -105
			E(userId, housing,       "Rent",                  1650.00m, today, -118),
			E(userId, shopping,      "Amazon Prime",           139.00m, today, -118),
			E(userId, groceries,     "Kroger",                  87.43m, today, -117),
			E(userId, diningOut,     "Chipotle",                14.25m, today, -115),
			E(userId, transport,     "Shell Gas",               52.10m, today, -114),
			E(userId, utilities,     "Electric Bill",           85.00m, today, -113),
			E(userId, utilities,     "Internet",                59.99m, today, -113),
			E(userId, housing,       "Renters Insurance",       18.00m, today, -112),
			E(userId, entertainment, "Spotify",                 11.99m, today, -111),
			E(userId, entertainment, "Netflix",                 15.49m, today, -111),
			E(userId, health,        "Gym",                     35.00m, today, -111),
			E(userId, groceries,     "Walmart",                 63.17m, today, -108),
			E(userId, diningOut,     "Panera",                  11.80m, today, -106),

			// Period 2: day -104 to -91
			E(userId, groceries,     "Kroger",                  91.22m, today, -103),
			E(userId, shopping,      "Amazon",                  34.99m, today, -102),
			E(userId, transport,     "Shell Gas",               48.75m, today, -101),
			E(userId, diningOut,     "Olive Garden",            38.50m, today, -100),
			E(userId, shopping,      "Target",                  52.30m, today,  -98),
			E(userId, health,        "Walgreens",               18.45m, today,  -96),
			E(userId, groceries,     "Kroger",                  55.60m, today,  -94),
			E(userId, diningOut,     "McDonald's",               9.45m, today,  -92),

			// Period 3: day -90 to -77
			E(userId, housing,       "Rent",                  1650.00m, today,  -90),
			E(userId, transport,     "Car Insurance (6-month)", 700.00m, today,  -90),
			E(userId, utilities,     "Electric Bill",           91.00m, today,  -89),
			E(userId, utilities,     "Internet",                59.99m, today,  -89),
			E(userId, housing,       "Renters Insurance",       18.00m, today,  -89),
			E(userId, entertainment, "Spotify",                 11.99m, today,  -88),
			E(userId, entertainment, "Netflix",                 15.49m, today,  -88),
			E(userId, health,        "Gym",                     35.00m, today,  -88),
			E(userId, groceries,     "Kroger",                  78.34m, today,  -86),
			E(userId, transport,     "Shell Gas",               50.20m, today,  -85),
			E(userId, diningOut,     "Chick-fil-A",             13.75m, today,  -83),
			E(userId, shopping,      "Amazon",                  67.49m, today,  -81),
			E(userId, groceries,     "Kroger",                  44.12m, today,  -79),
			E(userId, health,        "Doctor Copay",            30.00m, today,  -78),

			// Period 4: day -76 to -63
			E(userId, diningOut,     "Valentine's Dinner",      94.20m, today,  -76),
			E(userId, groceries,     "Kroger",                  82.55m, today,  -75),
			E(userId, transport,     "Shell Gas",               47.90m, today,  -73),
			E(userId, shopping,      "Target",                  38.75m, today,  -71),
			E(userId, groceries,     "Kroger",                  61.30m, today,  -68),
			E(userId, entertainment, "Hulu",                    17.99m, today,  -67),
			E(userId, diningOut,     "Chipotle",                15.10m, today,  -65),
			E(userId, shopping,      "Amazon",                  29.99m, today,  -64),

			// Period 5: day -62 to -49
			E(userId, housing,       "Rent",                  1650.00m, today,  -62),
			E(userId, utilities,     "Electric Bill",           78.00m, today,  -61),
			E(userId, utilities,     "Internet",                59.99m, today,  -61),
			E(userId, housing,       "Renters Insurance",       18.00m, today,  -61),
			E(userId, entertainment, "Spotify",                 11.99m, today,  -60),
			E(userId, entertainment, "Netflix",                 15.49m, today,  -60),
			E(userId, health,        "Gym",                     35.00m, today,  -60),
			E(userId, groceries,     "Kroger",                  95.43m, today,  -58),
			E(userId, transport,     "Shell Gas",               53.60m, today,  -56),
			E(userId, diningOut,     "Starbucks",                8.75m, today,  -54),
			E(userId, groceries,     "Walmart",                 71.20m, today,  -52),
			E(userId, shopping,      "Amazon",                  44.99m, today,  -50),

			// Period 6: day -48 to -35
			E(userId, groceries,     "Kroger",                  88.10m, today,  -47),
			E(userId, transport,     "Shell Gas",               49.30m, today,  -45),
			E(userId, diningOut,     "Buffalo Wild Wings",      42.15m, today,  -44),
			E(userId, shopping,      "Target",                  61.45m, today,  -42),
			E(userId, groceries,     "Kroger",                  53.80m, today,  -39),
			E(userId, health,        "Pharmacy",                22.50m, today,  -38),
			E(userId, entertainment, "Hulu",                    17.99m, today,  -37),
			E(userId, diningOut,     "Chipotle",                14.50m, today,  -36),

			// Period 7: day -34 to -21
			E(userId, housing,       "Rent",                  1650.00m, today,  -34),
			E(userId, utilities,     "Electric Bill",           82.00m, today,  -33),
			E(userId, utilities,     "Internet",                59.99m, today,  -33),
			E(userId, housing,       "Renters Insurance",       18.00m, today,  -33),
			E(userId, entertainment, "Spotify",                 11.99m, today,  -32),
			E(userId, entertainment, "Netflix",                 15.49m, today,  -32),
			E(userId, health,        "Gym",                     35.00m, today,  -32),
			E(userId, groceries,     "Kroger",                  76.85m, today,  -30),
			E(userId, transport,     "Shell Gas",               51.40m, today,  -29),
			E(userId, diningOut,     "Panera",                  12.90m, today,  -27),
			E(userId, shopping,      "Amazon",                  89.99m, today,  -26),
			E(userId, groceries,     "Walmart",                 58.30m, today,  -24),

			// Period 8: day -20 to -7
			E(userId, groceries,     "Kroger",                  83.45m, today,  -20),
			E(userId, transport,     "Shell Gas",               46.80m, today,  -18),
			E(userId, diningOut,     "Olive Garden",            51.30m, today,  -17),
			E(userId, shopping,      "Target",                  44.20m, today,  -15),
			E(userId, groceries,     "Kroger",                  67.90m, today,  -12),
			E(userId, health,        "Doctor Copay",            30.00m, today,  -11),
			E(userId, entertainment, "Hulu",                    17.99m, today,  -10),
			E(userId, shopping,      "Amazon",                  33.49m, today,   -9),

			// Period 9: day -6 to 0 (today)
			E(userId, housing,       "Rent",                  1650.00m, today,   -6),
			E(userId, utilities,     "Electric Bill",           74.00m, today,   -5),
			E(userId, utilities,     "Internet",                59.99m, today,   -5),
			E(userId, housing,       "Renters Insurance",       18.00m, today,   -4),
			E(userId, entertainment, "Spotify",                 11.99m, today,   -3),
			E(userId, entertainment, "Netflix",                 15.49m, today,   -3),
			E(userId, health,        "Gym",                     35.00m, today,   -3),
			E(userId, groceries,     "Kroger",                  79.15m, today,   -1),
			E(userId, transport,     "Shell Gas",               50.60m, today,    0),
		];

		db.Expenses.AddRange(expenses);
		await db.SaveChangesAsync();
	}

	// -------------------------
	// Clear existing demo data
	// -------------------------
	private static async Task ClearDemoDataAsync(AppDbContext db, int userId)
	{
		db.Expenses.RemoveRange(db.Expenses.Where(e => e.UserId == userId));
		db.RecurringExpenses.RemoveRange(db.RecurringExpenses.Where(e => e.UserId == userId));
		db.PayChecks.RemoveRange(db.PayChecks.Where(e => e.UserId == userId));
		db.PaySchedules.RemoveRange(db.PaySchedules.Where(e => e.UserId == userId));
		db.ExpenseCategories.RemoveRange(db.ExpenseCategories.Where(e => e.UserId == userId));
		db.Users.RemoveRange(db.Users.Where(u => u.UserId == userId));
		await db.SaveChangesAsync();
	}

	// -------------------------
	// Date helper — offset from anchor date
	// -------------------------
	private static DateTime DateOffset(DateTime anchor, int offsetDays)
		=> anchor.AddDays(offsetDays);

	// -------------------------
	// Expense helper
	// -------------------------
	private static Expense E(int userId, ExpenseCategory category, string description, decimal amount, DateTime anchor, int offsetDays)
	{
		return new Expense
		{
			UserId = userId,
			ExpenseCategoryId = category.ExpenseCategoryId,
			Description = description,
			Amount = amount,
			Date = DateOffset(anchor, offsetDays)
		};
	}
}