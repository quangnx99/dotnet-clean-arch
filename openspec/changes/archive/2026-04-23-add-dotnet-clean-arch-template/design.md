# Design: add-dotnet-clean-arch-template

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                        API Layer                        │
│  Controllers ─► ISender (Mediator)                      │
│  Program.cs · Middleware · Swagger · HealthChecks       │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on
         ┌──────────────┴──────────────┐
         │                             │
         ▼                             ▼
┌────────────────────┐     ┌────────────────────────────┐
│  Application Layer │     │    Infrastructure Layer     │
│                    │     │                            │
│  Commands/Queries  │     │  ApplicationDbContext      │
│  ValidationBehav.  │     │  (EF Core + Npgsql)        │
│  LoggingBehavior   │     │                            │
│  ICacheService ────┼─────►  CacheService (Redis)      │
│  IUnitOfWork   ────┼─────►  UnitOfWork                │
│  IAppDbContext ────┼─────►  ApplicationDbContext       │
│                    │     │  JwtOptions / AddJwtAuth   │
└────────┬───────────┘     └────────────────────────────┘
         │ depends on
         ▼
┌────────────────────┐
│    Domain Layer    │
│                    │
│  Result<T>/Error   │
│  Entity base       │
│  AggregateRoot     │
│  IDomainEvent      │
│  Product aggregate │
│  ProductId (VO)    │
│  ProductErrors     │
└────────────────────┘
```

## Dependency Flow (strictly one-way)

```
Domain  ←  Application  ←  Infrastructure  ←  API
  ↑               ↑
UnitTests      IntegTests (via WebApplicationFactory + Testcontainers)
```

- **Domain**: zero external NuGet dependencies.
- **Application**: references Domain only; all infrastructure is behind interfaces.
- **Infrastructure**: implements Application abstractions; references EF Core, Npgsql,
  StackExchange.Redis, Serilog.
- **API**: references Application (for ISender + DI) and Infrastructure (for DI
  registration); controllers are thin dispatchers.

## Key Decision Log

### Mediator (martinothamar) vs MediatR
MediatR switched to a commercial licence for server-side use (LGPLv2 ambiguity →
commercial for production). `Mediator` by martinothamar is MIT, source-generated
(zero reflection overhead), AOT-compatible, and API-compatible with MediatR for
basic CQRS. Chosen to avoid licence risk and improve startup performance.

### Manual mapping extension methods vs AutoMapper / Mapster
AutoMapper requires runtime reflection and configuration; Mapster requires
compile-time code-gen setup and adds licence uncertainty for edge cases. Manual
`static ProductMappings` extension methods are trivial (<20 lines), compile-time
safe, AOT-clean, and entirely readable by a junior dev. No hidden magic.

### Swashbuckle vs Scalar
Scalar is newer but still maturing; Swashbuckle.AspNetCore 6.x is the established
ecosystem standard with broad IDE/codegen tool support. Swashbuckle ships working
Swagger UI out of the box and integrates with XML doc comments trivially. Chosen
for stability and ecosystem breadth.

### No ASP.NET Identity
Identity bundles opinionated DB schema, Cookie auth, and Razor Pages scaffolding
that conflicts with clean-arch patterns. Template provides JWT bearer config
skeleton (`JwtOptions`, `AddJwtAuth`) that consumers wire to any identity provider
(Keycloak, Auth0, custom user service). Keeps template minimal and unambiguous.

### Custom Result<T> vs ErrorOr
`ErrorOr` is a single-file NuGet — excellent library, but adding an external
dependency just for Result defeats the "zero deps on Domain" principle. Custom
`Result<T>` + `Error` + `ErrorType` is ~80 lines, fully understood, zero NuGet,
and directly matchable in controllers via `ResultExtensions`.

### Central Package Management (Directory.Packages.props)
Prevents version drift across multi-project solutions. All version pins live in
one file; projects use `<PackageReference Include="..." />` with no `Version`
attribute. Makes dependabot PRs affect one file.

### PostgreSQL 16 + Redis 7 in docker-compose
Both are current LTS/stable. Alpine images minimise footprint. Named volumes
ensure data persists across `docker compose restart`. Health-check probes prevent
API starting before databases are ready.

## Rename Strategy (for Claude Code Skill)

The skill must rename the template to a consumer-supplied `<NewName>`. Strategy:

1. **File/folder rename** — recursively walk tree; rename any file or directory
   whose name contains `DotnetCleanArch` → `<NewName>` (depth-first, rename
   leaves before parents).
2. **Content substitution** — for every text file (`.cs`, `.csproj`, `.sln`,
   `.md`, `.yml`, `.yaml`, `.json`, `.props`, `.targets`, `.env*`, `.editorconfig`,
   `.gitignore`), replace all occurrences of:
   - `DotnetCleanArch` → `<NewName>`
   - `dotnet-clean-arch` → `<new-name-kebab>` (lowercase hyphen form)
   - `<your-username>/dotnet-clean-arch` → `<owner>/<new-name-kebab>`
3. `.git` directory is deleted and re-initialised after rename to avoid carrying
   template history.
4. All substitutions use PowerShell `Get-ChildItem -Recurse` + `-replace` operator
   or equivalent Claude Code built-in file tools — no reliance on bash/sed/awk
   for Windows compatibility.

## File Structure (final)

```
D:\Projects\dotnet-starter\
├── .editorconfig
├── .env.example
├── .gitignore
├── .dockerignore
├── Directory.Build.props
├── Directory.Packages.props
├── DotnetCleanArch.sln
├── Dockerfile
├── LICENSE
├── README.md
├── CONTRIBUTING.md
├── docker-compose.yml
├── global.json
├── .github/
│   ├── dependabot.yml
│   ├── pull_request_template.md
│   └── workflows/
│       └── ci.yml
├── src/
│   ├── DotnetCleanArch.Domain/
│   │   ├── DotnetCleanArch.Domain.csproj
│   │   ├── Common/
│   │   │   ├── Result.cs
│   │   │   ├── ResultT.cs
│   │   │   ├── Error.cs
│   │   │   └── ErrorType.cs
│   │   ├── Primitives/
│   │   │   ├── Entity.cs
│   │   │   ├── AggregateRoot.cs
│   │   │   └── IDomainEvent.cs
│   │   └── Products/
│   │       ├── Product.cs
│   │       ├── ProductId.cs
│   │       └── Errors/
│   │           └── ProductErrors.cs
│   ├── DotnetCleanArch.Application/
│   │   ├── DotnetCleanArch.Application.csproj
│   │   ├── DependencyInjection.cs
│   │   ├── Abstractions/
│   │   │   ├── Messaging/
│   │   │   │   ├── ICommand.cs
│   │   │   │   ├── ICommandHandler.cs
│   │   │   │   ├── IQuery.cs
│   │   │   │   └── IQueryHandler.cs
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   └── LoggingBehavior.cs
│   │   │   ├── Caching/
│   │   │   │   └── ICacheService.cs
│   │   │   └── Data/
│   │   │       ├── IUnitOfWork.cs
│   │   │       └── IApplicationDbContext.cs
│   │   └── Products/
│   │       ├── Commands/
│   │       │   └── CreateProduct/
│   │       │       ├── CreateProductCommand.cs
│   │       │       ├── CreateProductCommandHandler.cs
│   │       │       └── CreateProductCommandValidator.cs
│   │       ├── Queries/
│   │       │   └── GetProductById/
│   │       │       ├── GetProductByIdQuery.cs
│   │       │       ├── GetProductByIdQueryHandler.cs
│   │       │       └── GetProductByIdResponse.cs
│   │       └── Mappings/
│   │           └── ProductMappings.cs
│   ├── DotnetCleanArch.Infrastructure/
│   │   ├── DotnetCleanArch.Infrastructure.csproj
│   │   ├── DependencyInjection.cs
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── UnitOfWork.cs
│   │   │   ├── Configurations/
│   │   │   │   └── ProductConfiguration.cs
│   │   │   ├── Interceptors/
│   │   │   │   └── UpdateAuditableEntitiesInterceptor.cs
│   │   │   └── Migrations/
│   │   │       └── .gitkeep
│   │   ├── Caching/
│   │   │   └── CacheService.cs
│   │   └── Authentication/
│   │       ├── JwtOptions.cs
│   │       └── JwtServiceExtensions.cs
│   └── DotnetCleanArch.Api/
│       ├── DotnetCleanArch.Api.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Controllers/
│       │   ├── ApiController.cs
│       │   └── ProductsController.cs
│       ├── Middleware/
│       │   └── GlobalExceptionHandler.cs
│       ├── Extensions/
│       │   └── ResultExtensions.cs
│       └── OpenApi/
│           └── ConfigureSwaggerOptions.cs
└── tests/
    ├── DotnetCleanArch.UnitTests/
    │   ├── DotnetCleanArch.UnitTests.csproj
    │   ├── Domain/
    │   │   ├── ResultTests.cs
    │   │   └── ProductTests.cs
    │   └── Application/
    │       ├── CreateProductCommandHandlerTests.cs
    │       └── GetProductByIdQueryHandlerTests.cs
    └── DotnetCleanArch.IntegrationTests/
        ├── DotnetCleanArch.IntegrationTests.csproj
        ├── Infrastructure/
        │   └── WebAppFactory.cs
        └── Products/
            └── ProductsEndpointTests.cs
```
