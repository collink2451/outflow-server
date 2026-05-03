# Outflow — Backend

ASP.NET Core 9 API for [Outflow](https://outflow.collinkoldoff.dev), a personal budgeting app. Handles authentication, data persistence, and pay period calculations.

## Stack

- **.NET 9 ASP.NET Core Web API**
- **Entity Framework Core 9 + Pomelo MySQL**
- **Google OAuth** via cookie-based authentication
- **Rate limiting** — 30 requests/minute per IP

## Features

- Google OAuth login with automatic user provisioning
- Full CRUD for expenses, categories, pay schedules, and pay checks
- Pay periods computed server-side: expenses are bucketed by pay check date ranges
- Demo account with pre-seeded data, auto-reset daily at 8 AM UTC
- OpenAPI docs available in development at `/openapi/v1.json`

## Local Development

### Prerequisites

- .NET 9 SDK
- MySQL instance

### Setup

Store the connection string in user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=outflow;User=...;Password=...;"
dotnet user-secrets set "Authentication:Google:ClientId" "<your-client-id>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<your-client-secret>"
dotnet user-secrets set "FrontendUrl" "http://localhost:4200"
```

### Run

```bash
dotnet run --project Outflow.Server
```

API listens on `http://localhost:5257`. The Angular dev proxy forwards `/api` and `/auth` requests here.

### Migrations

```bash
dotnet ef database update --project Outflow.Server
```

## API Routes

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/auth/login` | Redirect to Google OAuth |
| GET | `/auth/callback` | OAuth callback, sets auth cookie |
| GET | `/auth/demo` | Sign in as demo user |
| GET | `/auth/logout` | Clear auth cookie |
| GET | `/auth/me` | Current user info |
| GET/PUT/DELETE | `/api/users/me` | User profile |
| GET | `/api/frequencies` | Frequency options (Weekly, BiWeekly, etc.) |
| GET/POST/PUT/DELETE | `/api/expensecategories` | Expense categories |
| GET/POST/PUT/DELETE | `/api/expenses` | Expenses |
| GET/POST/PUT/DELETE | `/api/recurringexpenses` | Recurring expenses |
| GET/POST/PUT/DELETE | `/api/payschedules` | Pay schedules |
| GET/POST/PUT/DELETE | `/api/paychecks` | Pay checks |
| GET | `/api/payperiods` | Computed pay periods with bucketed expenses |

> Note: ASP.NET Core route names do not include hyphens — `ExpenseCategoriesController` maps to `/api/expensecategories`.

## Project Structure

```
Outflow.Server/
├── Controllers/     One controller per resource + ApiControllerBase
├── Data/            AppDbContext, design-time factory, DemoDataSeeder
├── DTOs/            Request/response shapes per resource
├── Models/          EF Core entities (BaseEntity, User, Expense, etc.)
├── Services/        DemoResetService (hosted service)
├── Migrations/      EF Core migrations
└── Program.cs       Service registration and middleware pipeline
```

## Deployment

Pushes to `master` trigger a GitHub Actions workflow that:

1. Restores, builds, and publishes the app
2. Bundles EF Core migrations
3. SCPs artifacts to `/var/www/outflow/server` on the Linux server
4. Runs migrations against the production database
5. Restarts the `outflow-server` systemd service

Required secrets: `SSH_HOST`, `SSH_USER`, `SSH_PRIVATE_KEY`, `DB_CONNECTION_STRING`.
