# Copilot Instructions for MedRevnu Codebase

## Architecture Overview
- **Modular monolith** built on ASP.NET Zero 13.4.0, ABP Framework 9.4.2, .NET 8.0
- **Layered structure:**
  - Presentation: `src/ATI.Web.Mvc`, `src/ATI.Web.Host`, `src/ATI.Web.Public`, `src/ATI.Maui`
  - Application: `src/ATI.Application`, `*.Application` modules
  - Domain: `src/ATI.Core`, `*.Domain` modules
  - Infrastructure: `src/ATI.EntityFrameworkCore`, `*.EntityFrameworkCore` modules
- **Bounded contexts:** Core, Admin, MedRevenue, Pharmacy (each with Domain, Application, EntityFrameworkCore, Web)
- **Module integration:** ABP `[DependsOn]` attributes; all modules share `ATI.Core`

## Developer Workflows
- **Build all:** `dotnet build ATI.All.sln`
- **Build web:** `dotnet build ATI.Web.sln`
- **Run MVC:** `cd src/ATI.Web.Mvc && dotnet run`
- **Run API:** `cd src/ATI.Web.Host && dotnet run`
- **Frontend build:** `cd src/ATI.Web.Mvc && yarn && yarn create-bundles` (dev) or `yarn build` (prod)
- **Test:** `dotnet test test/ATI.Tests/ATI.Tests.csproj`
- **GraphQL test:** `dotnet test test/ATI.GraphQL.Tests/ATI.GraphQL.Tests.csproj`
- **Migrations:**
  - Create: `cd src/ATI.EntityFrameworkCore && dotnet ef migrations add Name --startup-project ../ATI.Web.Mvc`
  - Run: `cd src/ATI.Migrator && dotnet run`

## Project Conventions
- **Module pattern:** Each feature = `{Module}.Domain`, `{Module}.Application`, `{Module}.EntityFrameworkCore`, `{Module}.Web`
- **Entities:** In Domain, registered in `ATIDbContext`, migrations via EF Core
- **Application services:** Interface in `.Application.Shared`, implementation in `.Application`, auto-exposed as API
- **Multi-tenancy:** Always use `IRepository<T>` for tenant filtering; context via subdomain/header/cookie
- **Authorization:** Use `[AbpAuthorize]` and define permissions in `AppPermissions`
- **Frontend:** Razor views (not SPA); assets managed with Gulp/yarn; static in `wwwroot/view-resources/`

## Integration & Dependencies
- **Authentication:** OpenIddict (OAuth2/OIDC), JWT
- **Database:** SQL Server, multi-schema (e.g., "ADM" for Admin)
- **Background jobs:** Hangfire (optional)
- **Caching:** Redis (optional)
- **Payments:** Stripe, PayPal
- **Email/SMS:** MailKit, Twilio
- **Logging:** Log4Net

## Key Files & Directories
- **Solutions:** `ATI.All.sln`, `ATI.Web.sln`, `ATI.Mobile.sln`
- **Entry:** `src/ATI.Web.Mvc/Startup/Program.cs`, `Startup.cs`
- **DbContext:** `src/ATI.EntityFrameworkCore/EntityFrameworkCore/ATIDbContext.cs`
- **Config:** `src/ATI.Web.Mvc/appsettings.json`, `common.props`
- **Tests:** `test/ATI.Tests`, `test/ATI.GraphQL.Tests`

## API & URLs
- **Web API:** `src/ATI.Web.Host` (`/api/services/app/{Service}/{Action}`)
- **Swagger:** `/swagger`
- **GraphQL:** `/graphql` (if enabled)
- **SignalR:** `/signalr`, `/signalr-chat`
- **Default URLs:** https://localhost:44302 (MVC), https://localhost:44301 (API)

## Patterns & Gotchas
- **Always consider tenant context in queries**
- **Register new modules in solution, DbContext, and via `[DependsOn]`**
- **Frontend is server-rendered, not SPA**
- **Use ABP/Zero conventions for services, permissions, and integration**

Refer to `CLAUDE.md` for detailed architecture and workflow explanations. For new modules or features, follow the established folder and naming patterns.
