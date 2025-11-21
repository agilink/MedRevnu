# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an **ASP.NET Zero 13.4.0** application built on **ABP Framework 9.4.2** using **.NET 8.0**. It follows a modular monolithic architecture with Domain-Driven Design (DDD) principles and multi-tenancy support.

## Solution Files

- **ATI.All.sln** - Complete solution with all projects
- **ATI.Web.sln** - Web-focused solution (Admin + MedRevenue modules)
- **ATI.Mobile.sln** - Mobile application solution (.NET MAUI)

## Architecture

### Layered Structure

The application follows a strict layered architecture:

```
Presentation (ATI.Web.Mvc, ATI.Web.Host, ATI.Web.Public, ATI.Maui)
    ↓
Application (ATI.Application, *.Application modules)
    ↓
Domain (ATI.Core, *.Domain modules)
    ↓
Infrastructure (ATI.EntityFrameworkCore, *.EntityFrameworkCore modules)
```

### Bounded Contexts

The system is organized into multiple bounded contexts:

1. **Core (src/ATI.Core)** - Main domain with multi-tenancy, authentication, authorization, subscriptions, and payments
2. **Admin Module (Admin/)** - Company, Facility, Address management (uses "ADM" database schema)
3. **MedRevenue Module (MedRevenue/)** - Revenue cycle management
4. **Pharmacy Module (Pharmacy/)** - Pharmacy-specific functionality (experimental)

Each module follows the pattern:
- `{Module}.Domain` - Domain entities and business logic
- `{Module}.Application` - Application services and DTOs
- `{Module}.EntityFrameworkCore` - EF Core configuration and repositories
- `{Module}.Web` - Web-specific configuration and integration

### Module Integration

Modules integrate via ABP's `[DependsOn]` attribute system. Example:

```csharp
[DependsOn(
    typeof(ATIWebCoreModule),
    typeof(RevenueWebModule)
)]
public class ATIWebMvcModule : AbpModule
```

All modules share a common kernel via `ATI.Core` and `ATI.Core.Shared`.

## Build Commands

### Backend

```bash
# Build entire solution
dotnet build ATI.All.sln

# Build web solution only
dotnet build ATI.Web.sln

# Build in Release mode
dotnet build -c Release

# Run MVC application
cd src\ATI.Web.Mvc
dotnet run

# Run Web API
cd src\ATI.Web.Host
dotnet run
```

### Frontend

The MVC application uses Gulp for frontend asset management:

```bash
cd src\ATI.Web.Mvc

# Install dependencies
yarn

# Development build (non-minified, with source maps)
yarn create-bundles

# Production build (minified and optimized)
yarn build
```

## Testing

```bash
# Run all unit tests
dotnet test test\ATI.Tests\ATI.Tests.csproj

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run GraphQL tests
dotnet test test\ATI.GraphQL.Tests\ATI.GraphQL.Tests.csproj
```

Test projects:
- `test/ATI.Tests` - Main unit tests (xUnit)
- `test/ATI.Test.Base` - Base test infrastructure
- `test/ATI.GraphQL.Tests` - GraphQL API tests

## Database Migrations

### Creating Migrations

```bash
cd src\ATI.EntityFrameworkCore

# Create new migration
dotnet ef migrations add MigrationName --startup-project ..\ATI.Web.Mvc
```

### Running Migrations

The `ATI.Migrator` console application handles database migrations for both host and all tenant databases:

```bash
cd src\ATI.Migrator
dotnet run
```

Migrations are stored in `src/ATI.EntityFrameworkCore/Migrations/`.

### DbContext

- **Main DbContext**: `ATIDbContext` in `src/ATI.EntityFrameworkCore/EntityFrameworkCore/`
- Inherits from `AbpZeroDbContext<Tenant, Role, User, ATIDbContext>`
- Implements `IOpenIddictDbContext` for OAuth/OIDC
- Supports multi-schema (e.g., "ADM" for Admin module)
- Connection string in `appsettings.json` → `ConnectionStrings:Default`

## API Structure

The application supports both Web API and MVC:

### Web API (REST)

- **Host**: `src/ATI.Web.Host` (standalone API server)
- **Authentication**: JWT Bearer tokens
- **Documentation**: Swagger UI at `/swagger`
- **Route Pattern**: `/api/services/{app|host}/{service-name}/{action}`

ABP automatically exposes Application Services as Web API controllers. For example:

```csharp
public class CompanyAppService : ApplicationService
{
    // Auto-generated endpoints:
    // GET /api/services/app/Company/Get
    // POST /api/services/app/Company/Create
}
```

### MVC Application

- **Host**: `src/ATI.Web.Mvc` (server-rendered UI with embedded API)
- **Views**: Razor views and pages (NOT Angular or React)
- **Route Pattern**: `{area}/{controller}/{action}/{id?}`
- **Areas**: Organized by module (e.g., `Areas/Core/`)

### GraphQL (Optional)

- **Project**: `src/ATI.GraphQL`
- **Configuration**: `appsettings.json` → `GraphQL:Enabled`
- **Endpoint**: `/graphql`
- **Playground**: `/ui/playground` (when enabled)

### Real-time

SignalR hubs available at:
- `/signalr` - Common hub (notifications)
- `/signalr-chat` - Chat functionality

## Frontend Technology

The application uses **ASP.NET MVC with Razor views** (server-rendered), NOT a SPA framework.

### Key Frontend Libraries

- Bootstrap 5.3.3
- jQuery 3.7.1
- DataTables 2.1.6
- Select2
- Moment.js
- Chart.js 4.4.4
- SignalR client
- SweetAlert2

### Frontend Structure

```
src/ATI.Web.Mvc/
├── Areas/              # Module-specific areas
├── Controllers/        # MVC controllers
├── Views/              # Razor views
├── wwwroot/           # Static assets
│   ├── view-resources/  # Page-specific JS/CSS
│   └── lib/            # Vendor libraries
├── package.json       # Frontend dependencies
└── gulpfile.js        # Build tasks
```

## Development Workflow

### Adding a New Module

1. Create projects following the pattern:
   - `{Module}.Domain`
   - `{Module}.Application`
   - `{Module}.EntityFrameworkCore`
   - `{Module}.Web`

2. Reference the module in `ATI.Web.Mvc.csproj`

3. Add `[DependsOn(typeof({Module}WebModule))]` to `ATIWebMvcModule`

4. Register entities in `ATIDbContext`

5. Generate and run migrations

### Adding Database Entities

1. Create entity class in `{Module}.Domain`
2. Add `DbSet<Entity>` to `ATIDbContext`
3. Configure entity mapping in `OnModelCreating()` if needed
4. Generate migration: `dotnet ef migrations add EntityName --startup-project ..\ATI.Web.Mvc`
5. Run migrator: `cd src\ATI.Migrator && dotnet run`

### Creating Application Services

1. Define interface in `{Module}.Application.Shared`
2. Implement service in `{Module}.Application`
3. ABP automatically exposes as Web API at `/api/services/app/{ServiceName}/{Method}`
4. Create MVC controller in `{Module}.Web` if custom UI is needed

## Key Technologies

- **Backend**: .NET 8.0, ASP.NET Core MVC, Entity Framework Core 8.0.8
- **Framework**: ASP.NET Zero 13.4.0, ABP Framework 9.4.2
- **Authentication**: OpenIddict 5.8.0 (OAuth 2.0/OIDC), JWT Bearer
- **Database**: SQL Server (multi-tenant with separate schemas)
- **Background Jobs**: Hangfire (optional)
- **Caching**: Redis (configurable)
- **Logging**: Log4Net
- **Mapping**: AutoMapper
- **IoC**: Castle Windsor (via ABP)
- **Payments**: Stripe, PayPal
- **Email**: MailKit
- **SMS**: Twilio

## Multi-Tenancy

Multi-tenancy is enabled by default:
- Each tenant can have a separate database or shared database with tenant filtering
- Tenant resolution via subdomain, header, or cookie
- Always consider tenant context when writing queries
- Use `IRepository<T>` which automatically filters by current tenant

## Authorization

Permission-based authorization system:
- Permissions defined in `AppPermissions` class
- Applied via `[AbpAuthorize(PermissionNames.Permission_Name)]` attribute
- Hierarchical permission structure
- Checked automatically in Application Services and Controllers

## Important Files

- **Entry Points**: `src/ATI.Web.Mvc/Startup/Program.cs`, `Startup.cs`
- **Configuration**: `src/ATI.Web.Mvc/appsettings.json`
- **Core Module**: `src/ATI.Core/ATICoreModule.cs`
- **DbContext**: `src/ATI.EntityFrameworkCore/EntityFrameworkCore/ATIDbContext.cs`
- **Shared Properties**: `common.props` (build configuration)

## Default URLs

- **MVC Application**: https://localhost:44302
- **Web API**: https://localhost:44301
- **Swagger UI**: https://localhost:44302/swagger or https://localhost:44301/swagger
- **GraphQL**: https://localhost:44302/graphql (if enabled)
