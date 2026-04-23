# Tasks: add-dotnet-clean-arch-template

All paths are relative to `D:\Projects\dotnet-starter\` unless prefixed otherwise.

---

## Group 1 — Solution Scaffolding

- [ ] 1.1 Create `global.json` at repo root:
  ```json
  {
    "sdk": { "version": "8.0.0", "rollForward": "latestFeature" }
  }
  ```

- [ ] 1.2 Create `Directory.Build.props` at repo root:
  ```xml
  <Project>
    <PropertyGroup>
      <TargetFramework>net8.0</TargetFramework>
      <Nullable>enable</Nullable>
      <ImplicitUsings>enable</ImplicitUsings>
      <LangVersion>latest</LangVersion>
      <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
      <AnalysisLevel>latest</AnalysisLevel>
      <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    </PropertyGroup>
  </Project>
  ```

- [ ] 1.3 Create `Directory.Packages.props` at repo root with `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>` and `<PackageVersion>` entries for every package used across all projects:
  - `Mediator` 2.1.x
  - `Mediator.Abstractions` 2.1.x
  - `FluentValidation` 11.x
  - `FluentValidation.DependencyInjectionExtensions` 11.x
  - `Microsoft.EntityFrameworkCore` 8.x
  - `Microsoft.EntityFrameworkCore.Design` 8.x
  - `Npgsql.EntityFrameworkCore.PostgreSQL` 8.x
  - `Microsoft.Extensions.Caching.StackExchangeRedis` 8.x
  - `Serilog.AspNetCore` 8.x
  - `Serilog.Sinks.Console` 5.x
  - `Serilog.Sinks.File` 5.x
  - `Serilog.Sinks.Seq` 7.x
  - `Swashbuckle.AspNetCore` 6.x
  - `Microsoft.AspNetCore.Authentication.JwtBearer` 8.x
  - `AspNetCore.HealthChecks.NpgSql` 8.x
  - `AspNetCore.HealthChecks.Redis` 8.x
  - `AspNetCore.HealthChecks.UI.Client` 8.x
  - `xunit` 2.9.x
  - `xunit.runner.visualstudio` 2.8.x
  - `Microsoft.NET.Test.Sdk` 17.x
  - `FluentAssertions` 6.x
  - `NSubstitute` 5.x
  - `Testcontainers` 3.x
  - `Testcontainers.PostgreSql` 3.x
  - `Testcontainers.Redis` 3.x
  - `Microsoft.AspNetCore.Mvc.Testing` 8.x
  - `coverlet.collector` 6.x

- [ ] 1.4 Create `.editorconfig` at repo root with:
  - `[*.cs]` section: `indent_size = 4`, `charset = utf-8-bom`, `end_of_line = crlf`
  - `csharp_style_namespace_declarations = file_scoped:warning`
  - `csharp_prefer_braces = true:warning`
  - `dotnet_sort_system_directives_first = true`
  - `dotnet_separate_import_directive_groups = false`
  - Full `[*.{yml,yaml,json}]` section: `indent_size = 2`

- [ ] 1.5 Create `.gitignore` at repo root covering: `bin/`, `obj/`, `*.user`,
  `*.suo`, `.vs/`, `.idea/`, `.rider/`, `*.DotSettings`, `*.log`, `*.env` (but not
  `.env.example`), `TestResults/`, `coverage*/`.

- [ ] 1.6 Create the solution file `DotnetCleanArch.sln` using
  `dotnet new sln -n DotnetCleanArch` (or write manually). It must reference all
  six projects (added in later steps).

- [ ] 1.7 Create `src/` and `tests/` directories (they will be populated by
  subsequent tasks).

---

## Group 2 — Domain Layer

- [ ] 2.1 Create `src/DotnetCleanArch.Domain/DotnetCleanArch.Domain.csproj`:
  - `<TargetFramework>` inherited from `Directory.Build.props`
  - No `<PackageReference>` entries (zero external deps)

- [ ] 2.2 Create `src/DotnetCleanArch.Domain/Common/ErrorType.cs`:
  ```csharp
  namespace DotnetCleanArch.Domain.Common;
  public enum ErrorType { Failure, Validation, NotFound, Conflict, Unauthorized }
  ```

- [ ] 2.3 Create `src/DotnetCleanArch.Domain/Common/Error.cs`:
  - Record struct with properties: `Code (string)`, `Description (string)`,
    `Type (ErrorType)`
  - Static factories: `Failure`, `Validation`, `NotFound`, `Conflict`,
    `Unauthorized`
  - Static `None` sentinel

- [ ] 2.4 Create `src/DotnetCleanArch.Domain/Common/Result.cs` (non-generic):
  - Properties: `bool IsSuccess`, `bool IsFailure`, `Error Error`
  - Static factories: `Success()`, `Failure(Error)`
  - Implicit operator from `Error`

- [ ] 2.5 Create `src/DotnetCleanArch.Domain/Common/ResultT.cs` (file name
  `Result.cs` in subfolder or `ResultT.cs` at same level — use `ResultT.cs` to
  avoid naming collision):
  - `Result<T>` with properties: `bool IsSuccess`, `T? Value`, `Error Error`
  - Static factories: `Success(T value)`, `Failure(Error)`
  - Implicit operators: from `T` → success, from `Error` → failure

- [ ] 2.6 Create `src/DotnetCleanArch.Domain/Primitives/IDomainEvent.cs`:
  - Marker interface, implement `INotification` from Mediator.Abstractions

- [ ] 2.7 Create `src/DotnetCleanArch.Domain/Primitives/Entity.cs`:
  - Abstract base with `protected` constructor
  - `Id` property of generic type `TId`
  - Internal `_domainEvents` list; public `IReadOnlyCollection<IDomainEvent> DomainEvents`
  - `protected void RaiseDomainEvent(IDomainEvent e)`
  - `public void ClearDomainEvents()`

- [ ] 2.8 Create `src/DotnetCleanArch.Domain/Primitives/AggregateRoot.cs`:
  - Inherits `Entity<TId>` — marks the aggregate boundary, no additional members
    needed in template (comment explaining purpose)

- [ ] 2.9 Create `src/DotnetCleanArch.Domain/Products/ProductId.cs`:
  - `readonly record struct ProductId(Guid Value)`
  - Static `New()` factory calling `Guid.NewGuid()`

- [ ] 2.10 Create `src/DotnetCleanArch.Domain/Products/Product.cs`:
  - Inherits `AggregateRoot<ProductId>`
  - Properties: `Name (string)`, `Description (string)`, `Price (decimal)`,
    `CreatedAt (DateTime)`, `UpdatedAt (DateTime?)`
  - Private constructor (for EF) + private constructor with all fields
  - Static factory `Create(string name, string description, decimal price)`
    returns `Result<Product>` — validates name not empty, price > 0
  - Method `Update(string name, string description, decimal price)` returns
    `Result` — same validation

- [ ] 2.11 Create `src/DotnetCleanArch.Domain/Products/Errors/ProductErrors.cs`:
  - Static class with static readonly `Error` fields:
    `NotFound`, `NameEmpty`, `InvalidPrice`, `AlreadyExists`

- [ ] 2.12 Add `src/DotnetCleanArch.Domain` project to `DotnetCleanArch.sln`.

---

## Group 3 — Application Layer

- [ ] 3.1 Create `src/DotnetCleanArch.Application/DotnetCleanArch.Application.csproj`:
  - `<ProjectReference>` to Domain
  - `<PackageReference Include="Mediator" />`
  - `<PackageReference Include="Mediator.Abstractions" />`
  - `<PackageReference Include="FluentValidation" />`
  - `<PackageReference Include="FluentValidation.DependencyInjectionExtensions" />`
  - `<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />`

- [ ] 3.2 Create `src/DotnetCleanArch.Application/Abstractions/Messaging/ICommand.cs`:
  ```csharp
  using Mediator;
  namespace DotnetCleanArch.Application.Abstractions.Messaging;
  public interface ICommand : IRequest<Result> { }
  public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
  ```

- [ ] 3.3 Create `src/DotnetCleanArch.Application/Abstractions/Messaging/ICommandHandler.cs`:
  ```csharp
  using Mediator;
  namespace DotnetCleanArch.Application.Abstractions.Messaging;
  public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
      where TCommand : ICommand { }
  public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
      where TCommand : ICommand<TResponse> { }
  ```

- [ ] 3.4 Create `src/DotnetCleanArch.Application/Abstractions/Messaging/IQuery.cs`:
  ```csharp
  using Mediator;
  namespace DotnetCleanArch.Application.Abstractions.Messaging;
  public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
  ```

- [ ] 3.5 Create `src/DotnetCleanArch.Application/Abstractions/Messaging/IQueryHandler.cs`:
  ```csharp
  using Mediator;
  namespace DotnetCleanArch.Application.Abstractions.Messaging;
  public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
      where TQuery : IQuery<TResponse> { }
  ```

- [ ] 3.6 Create `src/DotnetCleanArch.Application/Abstractions/Caching/ICacheService.cs`:
  ```csharp
  namespace DotnetCleanArch.Application.Abstractions.Caching;
  public interface ICacheService
  {
      Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
      Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
      Task RemoveAsync(string key, CancellationToken ct = default);
      Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory,
          TimeSpan? expiry = null, CancellationToken ct = default);
  }
  ```

- [ ] 3.7 Create `src/DotnetCleanArch.Application/Abstractions/Data/IUnitOfWork.cs`:
  ```csharp
  namespace DotnetCleanArch.Application.Abstractions.Data;
  public interface IUnitOfWork
  {
      Task<int> SaveChangesAsync(CancellationToken ct = default);
  }
  ```

- [ ] 3.8 Create `src/DotnetCleanArch.Application/Abstractions/Data/IApplicationDbContext.cs`:
  ```csharp
  using DotnetCleanArch.Domain.Products;
  using Microsoft.EntityFrameworkCore;
  namespace DotnetCleanArch.Application.Abstractions.Data;
  public interface IApplicationDbContext
  {
      DbSet<Product> Products { get; }
  }
  ```

- [ ] 3.9 Create `src/DotnetCleanArch.Application/Abstractions/Behaviors/ValidationBehavior.cs`:
  - `IPipelineBehavior<TMessage, TResponse>` from Mediator
  - Injects `IEnumerable<IValidator<TMessage>>`
  - Runs all validators, collects failures, returns `Result.Failure` aggregate
    error with `ErrorType.Validation` if any failures exist
  - Generic constraint: `where TResponse : IResult` (or use duck-typing via
    `Result` factory method — see note below on Result marker)

- [ ] 3.10 Create `src/DotnetCleanArch.Application/Abstractions/Behaviors/LoggingBehavior.cs`:
  - `IPipelineBehavior<TMessage, TResponse>`
  - Injects `ILogger<LoggingBehavior<TMessage, TResponse>>`
  - Logs request name + elapsed time at `Information`; logs warning if > 500 ms

- [ ] 3.11 Create `src/DotnetCleanArch.Application/Products/Commands/CreateProduct/CreateProductCommand.cs`:
  ```csharp
  public sealed record CreateProductCommand(string Name, string Description, decimal Price)
      : ICommand<ProductId>;
  ```

- [ ] 3.12 Create `src/DotnetCleanArch.Application/Products/Commands/CreateProduct/CreateProductCommandValidator.cs`:
  - `AbstractValidator<CreateProductCommand>`
  - `RuleFor(x => x.Name).NotEmpty().MaximumLength(200)`
  - `RuleFor(x => x.Price).GreaterThan(0)`

- [ ] 3.13 Create `src/DotnetCleanArch.Application/Products/Commands/CreateProduct/CreateProductCommandHandler.cs`:
  - `ICommandHandler<CreateProductCommand, ProductId>`
  - Injects `IApplicationDbContext`, `IUnitOfWork`
  - Calls `Product.Create(...)`, checks `IsFailure`, adds to `DbSet`, calls
    `SaveChangesAsync`, returns `Result<ProductId>.Success(product.Id)`

- [ ] 3.14 Create `src/DotnetCleanArch.Application/Products/Queries/GetProductById/GetProductByIdQuery.cs`:
  ```csharp
  public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<GetProductByIdResponse>;
  ```

- [ ] 3.15 Create `src/DotnetCleanArch.Application/Products/Queries/GetProductById/GetProductByIdResponse.cs`:
  ```csharp
  public sealed record GetProductByIdResponse(
      Guid Id, string Name, string Description, decimal Price,
      DateTime CreatedAt, DateTime? UpdatedAt);
  ```

- [ ] 3.16 Create `src/DotnetCleanArch.Application/Products/Queries/GetProductById/GetProductByIdQueryHandler.cs`:
  - `IQueryHandler<GetProductByIdQuery, GetProductByIdResponse>`
  - Injects `IApplicationDbContext`
  - Finds product by id, returns `ProductErrors.NotFound` if null, maps via
    `ProductMappings.ToResponse()`

- [ ] 3.17 Create `src/DotnetCleanArch.Application/Products/Mappings/ProductMappings.cs`:
  ```csharp
  public static class ProductMappings
  {
      public static GetProductByIdResponse ToResponse(this Product product) => new(
          product.Id.Value, product.Name, product.Description, product.Price,
          product.CreatedAt, product.UpdatedAt);
  }
  ```

- [ ] 3.18 Create `src/DotnetCleanArch.Application/DependencyInjection.cs`:
  - `AddApplication(this IServiceCollection services)` extension
  - `services.AddMediator(o => o.ServiceLifetime = ServiceLifetime.Scoped)`
  - `services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())`
  - Register `ValidationBehavior` and `LoggingBehavior` as pipeline behaviors

- [ ] 3.19 Add `src/DotnetCleanArch.Application` project to `DotnetCleanArch.sln`.

---

## Group 4 — Infrastructure Layer

- [ ] 4.1 Create `src/DotnetCleanArch.Infrastructure/DotnetCleanArch.Infrastructure.csproj`:
  - `<ProjectReference>` to Application
  - Package refs: `Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.Design`,
    `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.Extensions.Caching.StackExchangeRedis`,
    `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Sinks.File`,
    `Serilog.Sinks.Seq`, `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `<InternalsVisibleTo Include="DotnetCleanArch.IntegrationTests" />`

- [ ] 4.2 Create `src/DotnetCleanArch.Infrastructure/Persistence/ApplicationDbContext.cs`:
  - Inherits `DbContext`, implements `IApplicationDbContext`
  - Constructor takes `DbContextOptions<ApplicationDbContext>` + optional
    `IPublisher publisher` (Mediator) for domain event dispatch
  - `DbSet<Product> Products`
  - Override `SaveChangesAsync`: call base, then publish all collected domain
    events via `publisher.Publish()`
  - `OnModelCreating`: calls `base.OnModelCreating()`, then applies all
    `IEntityTypeConfiguration<>` from the assembly via
    `modelBuilder.ApplyConfigurationsFromAssembly()`

- [ ] 4.3 Create `src/DotnetCleanArch.Infrastructure/Persistence/Configurations/ProductConfiguration.cs`:
  - `IEntityTypeConfiguration<Product>`
  - Configure `ProductId` as owned entity or value converter to `Guid`
  - Table name `products`, column names snake_case
  - Name: max length 200, required
  - Price: `HasPrecision(18,2)`

- [ ] 4.4 Create `src/DotnetCleanArch.Infrastructure/Persistence/Interceptors/UpdateAuditableEntitiesInterceptor.cs`:
  - `SaveChangesInterceptor`
  - On `SavingChangesAsync`: iterate `ChangeTracker.Entries<Entity<...>>()` — for
    Added set `CreatedAt = DateTime.UtcNow`, for Modified set `UpdatedAt = DateTime.UtcNow`
  - Note: use `IAuditableEntity` marker interface on Product for clean filtering

- [ ] 4.5 Create `src/DotnetCleanArch.Infrastructure/Persistence/UnitOfWork.cs`:
  - Wraps `ApplicationDbContext`
  - Implements `IUnitOfWork`
  - `SaveChangesAsync` delegates to `_dbContext.SaveChangesAsync(ct)`

- [ ] 4.6 Create `src/DotnetCleanArch.Infrastructure/Persistence/Migrations/.gitkeep`
  (empty file to commit the folder; actual migration generated in Group 10).

- [ ] 4.7 Create `src/DotnetCleanArch.Infrastructure/Caching/CacheService.cs`:
  - Implements `ICacheService`
  - Injects `IDistributedCache`
  - `GetAsync<T>`: get bytes, deserialize with `System.Text.Json`; return default
    if null
  - `SetAsync<T>`: serialize with `System.Text.Json`, store with
    `DistributedCacheEntryOptions` using supplied expiry (default: no expiry)
  - `RemoveAsync`: `_cache.RemoveAsync(key, ct)`
  - `GetOrCreateAsync<T>`: try get; if null call factory, then set

- [ ] 4.8 Create `src/DotnetCleanArch.Infrastructure/Authentication/JwtOptions.cs`:
  ```csharp
  namespace DotnetCleanArch.Infrastructure.Authentication;
  public sealed class JwtOptions
  {
      public const string SectionName = "Jwt";
      public string Issuer { get; init; } = string.Empty;
      public string Audience { get; init; } = string.Empty;
      public string SecretKey { get; init; } = string.Empty;
      public int ExpiryMinutes { get; init; } = 60;
  }
  ```

- [ ] 4.9 Create `src/DotnetCleanArch.Infrastructure/Authentication/JwtServiceExtensions.cs`:
  - `AddJwtAuth(this IServiceCollection services, IConfiguration configuration)` extension
  - Binds `JwtOptions` from config section `"Jwt"`
  - Calls `services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)`
    with `TokenValidationParameters` populated from `JwtOptions`

- [ ] 4.10 Create `src/DotnetCleanArch.Infrastructure/DependencyInjection.cs`:
  - `AddInfrastructure(this IServiceCollection services, IConfiguration configuration)` extension
  - Register `ApplicationDbContext` with Npgsql provider; connection string from
    `configuration.GetConnectionString("Postgres")`
  - Register `UpdateAuditableEntitiesInterceptor` as singleton
  - Register `IUnitOfWork` → `UnitOfWork` scoped
  - Register `IApplicationDbContext` → `ApplicationDbContext` scoped (via factory or
    direct registration)
  - Register `ICacheService` → `CacheService` scoped
  - Call `services.AddStackExchangeRedisCache(o => o.Configuration = configuration.GetConnectionString("Redis"))`

- [ ] 4.11 Add `src/DotnetCleanArch.Infrastructure` project to `DotnetCleanArch.sln`.

---

## Group 5 — API Layer

- [ ] 5.1 Create `src/DotnetCleanArch.Api/DotnetCleanArch.Api.csproj`:
  - `<OutputType>Exe`
  - Project refs to Application + Infrastructure
  - Package refs: `Swashbuckle.AspNetCore`, `AspNetCore.HealthChecks.NpgSql`,
    `AspNetCore.HealthChecks.Redis`, `AspNetCore.HealthChecks.UI.Client`,
    `Serilog.AspNetCore`
  - `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
  - `<NoWarn>1591</NoWarn>` (suppress missing XML doc warnings for non-public
    members)

- [ ] 5.2 Create `src/DotnetCleanArch.Api/Controllers/ApiController.cs`:
  ```csharp
  [ApiController]
  [Route("api/[controller]")]
  public abstract class ApiController : ControllerBase
  {
      protected readonly ISender Sender;
      protected ApiController(ISender sender) => Sender = sender;
  }
  ```

- [ ] 5.3 Create `src/DotnetCleanArch.Api/Extensions/ResultExtensions.cs`:
  - `ToActionResult<T>(this Result<T> result)` → `Ok(result.Value)` on success
  - On failure, match `ErrorType`:
    - `NotFound` → `NotFound(ProblemDetails)`
    - `Validation` → `BadRequest(ValidationProblemDetails)`
    - `Conflict` → `Conflict(ProblemDetails)`
    - `Unauthorized` → `Unauthorized(ProblemDetails)`
    - `Failure` → `StatusCode(500, ProblemDetails)`
  - ProblemDetails populated with `Error.Code` and `Error.Description`

- [ ] 5.4 Create `src/DotnetCleanArch.Api/Middleware/GlobalExceptionHandler.cs`:
  - Implements `IExceptionHandler`
  - Catches `Exception`, logs via `ILogger`
  - Writes RFC 7807 `ProblemDetails` with `Status=500`,
    `Title="Internal Server Error"`, `Detail` omitted in non-Development

- [ ] 5.5 Create `src/DotnetCleanArch.Api/OpenApi/ConfigureSwaggerOptions.cs`:
  - Implements `IConfigureOptions<SwaggerGenOptions>`
  - Adds Bearer token security scheme and requirement
  - Includes XML comments file (from `GetXmlCommentsPath()` helper)

- [ ] 5.6 Create `src/DotnetCleanArch.Api/Controllers/ProductsController.cs`:
  ```csharp
  public sealed class ProductsController : ApiController
  {
      // GET api/products/{id}
      [HttpGet("{id:guid}")]
      public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
      {
          var result = await Sender.Send(new GetProductByIdQuery(id), ct);
          return result.ToActionResult();
      }

      // POST api/products
      [HttpPost]
      public async Task<IActionResult> Create(
          [FromBody] CreateProductCommand command, CancellationToken ct)
      {
          var result = await Sender.Send(command, ct);
          return result.Match(
              id => CreatedAtAction(nameof(GetById), new { id = id.Value }, null),
              e => e.ToActionResult());
      }
  }
  ```
  Note: add XML summary comments for Swagger.

- [ ] 5.7 Create `src/DotnetCleanArch.Api/Program.cs`:
  ```csharp
  // 1. Serilog bootstrap logger
  Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
  try
  {
      var builder = WebApplication.CreateBuilder(args);
      builder.Host.UseSerilog((ctx, lc) => lc
          .ReadFrom.Configuration(ctx.Configuration)
          .Enrich.FromLogContext());

      builder.Services
          .AddApplication()
          .AddInfrastructure(builder.Configuration)
          .AddJwtAuth(builder.Configuration);

      builder.Services.AddControllers();
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();
      builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
      builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
      builder.Services.AddProblemDetails();

      builder.Services
          .AddHealthChecks()
          .AddNpgSql(builder.Configuration.GetConnectionString("Postgres")!)
          .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

      var app = builder.Build();

      app.UseSerilogRequestLogging();
      app.UseExceptionHandler();

      if (app.Environment.IsDevelopment())
      {
          app.UseSwagger();
          app.UseSwaggerUI();
      }

      app.UseAuthentication();
      app.UseAuthorization();
      app.MapControllers();
      app.MapHealthChecks("/health", new HealthCheckOptions
      {
          ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
      });

      app.Run();
  }
  catch (Exception ex) when (ex is not HostAbortedException)
  {
      Log.Fatal(ex, "Application terminated unexpectedly");
  }
  finally { Log.CloseAndFlush(); }

  public partial class Program { } // for WebApplicationFactory
  ```

- [ ] 5.8 Create `src/DotnetCleanArch.Api/appsettings.json`:
  ```json
  {
    "Serilog": {
      "MinimumLevel": { "Default": "Information", "Override": { "Microsoft": "Warning" } },
      "WriteTo": [
        { "Name": "Console" },
        { "Name": "File", "Args": { "path": "logs/log-.txt", "rollingInterval": "Day" } }
      ]
    },
    "ConnectionStrings": {
      "Postgres": "${POSTGRES_CONNECTION_STRING}",
      "Redis": "${REDIS_CONNECTION_STRING}"
    },
    "Jwt": {
      "Issuer": "${JWT_ISSUER}",
      "Audience": "${JWT_AUDIENCE}",
      "SecretKey": "${JWT_SECRET}",
      "ExpiryMinutes": 60
    },
    "AllowedHosts": "*"
  }
  ```

- [ ] 5.9 Create `src/DotnetCleanArch.Api/appsettings.Development.json`:
  ```json
  {
    "Serilog": {
      "MinimumLevel": { "Default": "Debug", "Override": { "Microsoft.EntityFrameworkCore": "Information" } },
      "WriteTo": [
        { "Name": "Console" },
        { "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } }
      ]
    },
    "ConnectionStrings": {
      "Postgres": "Host=localhost;Port=5432;Database=dotnetcleanarch;Username=postgres;Password=postgres",
      "Redis": "localhost:6379"
    },
    "Jwt": {
      "Issuer": "dotnet-clean-arch",
      "Audience": "dotnet-clean-arch",
      "SecretKey": "dev-secret-key-change-in-production-32chars",
      "ExpiryMinutes": 1440
    }
  }
  ```

- [ ] 5.10 Add `src/DotnetCleanArch.Api` project to `DotnetCleanArch.sln`.

---

## Group 6 — Tests

### Unit Tests

- [ ] 6.1 Create `tests/DotnetCleanArch.UnitTests/DotnetCleanArch.UnitTests.csproj`:
  - `<IsPackable>false</IsPackable>`
  - Project ref to Domain + Application
  - Package refs: `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`,
    `FluentAssertions`, `NSubstitute`, `coverlet.collector`

- [ ] 6.2 Create `tests/DotnetCleanArch.UnitTests/Domain/ResultTests.cs`:
  - Test `Result.Success()` is success, not failure
  - Test `Result.Failure(error)` carries error type and code
  - Test `Result<T>.Success(value)` carries value
  - Test `Result<T>` implicit operator from `Error` produces failure

- [ ] 6.3 Create `tests/DotnetCleanArch.UnitTests/Domain/ProductTests.cs`:
  - Test `Product.Create` with valid data succeeds
  - Test `Product.Create` with empty name returns `ProductErrors.NameEmpty`
  - Test `Product.Create` with price ≤ 0 returns `ProductErrors.InvalidPrice`
  - Test `Product.Update` happy path
  - Test domain event raised after `Create`

- [ ] 6.4 Create `tests/DotnetCleanArch.UnitTests/Application/CreateProductCommandHandlerTests.cs`:
  - Mock `IApplicationDbContext`, `IUnitOfWork` with NSubstitute
  - Test successful creation returns `ProductId`
  - Test validation failure propagation

- [ ] 6.5 Create `tests/DotnetCleanArch.UnitTests/Application/GetProductByIdQueryHandlerTests.cs`:
  - Mock `IApplicationDbContext`
  - Test found product returns mapped response
  - Test not found returns `ProductErrors.NotFound`

- [ ] 6.6 Add `tests/DotnetCleanArch.UnitTests` to `DotnetCleanArch.sln`.

### Integration Tests

- [ ] 6.7 Create `tests/DotnetCleanArch.IntegrationTests/DotnetCleanArch.IntegrationTests.csproj`:
  - `<IsPackable>false</IsPackable>`
  - Project ref to Api
  - Package refs: `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`,
    `FluentAssertions`, `Testcontainers`, `Testcontainers.PostgreSql`,
    `Testcontainers.Redis`, `Microsoft.AspNetCore.Mvc.Testing`, `coverlet.collector`

- [ ] 6.8 Create `tests/DotnetCleanArch.IntegrationTests/Infrastructure/WebAppFactory.cs`:
  - `WebApplicationFactory<Program>` + `IAsyncLifetime`
  - Spins up `PostgreSqlContainer` and `RedisContainer` via Testcontainers
  - Overrides `ConnectionStrings:Postgres` and `ConnectionStrings:Redis` in
    `ConfigureWebHost`
  - Runs `dbContext.Database.MigrateAsync()` in `InitializeAsync`
  - Disposes containers in `DisposeAsync`

- [ ] 6.9 Create `tests/DotnetCleanArch.IntegrationTests/Products/ProductsEndpointTests.cs`:
  - `IClassFixture<WebAppFactory>`
  - `POST /api/products` with valid body → 201 Created
  - `POST /api/products` with empty name → 400 Bad Request
  - `GET /api/products/{id}` for existing product → 200 with response body
  - `GET /api/products/{id}` for unknown guid → 404

- [ ] 6.10 Add `tests/DotnetCleanArch.IntegrationTests` to `DotnetCleanArch.sln`.

---

## Group 7 — Docker

- [ ] 7.1 Create `.dockerignore` at repo root:
  ```
  **/.git
  **/.vs
  **/.idea
  **/bin
  **/obj
  **/TestResults
  **/*.user
  .env
  ```

- [ ] 7.2 Create `Dockerfile` at repo root (multi-stage):
  ```dockerfile
  # ── Build stage ──────────────────────────────────────
  FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
  WORKDIR /src
  COPY Directory.Build.props Directory.Packages.props global.json ./
  COPY src/ src/
  RUN dotnet restore src/DotnetCleanArch.Api/DotnetCleanArch.Api.csproj
  RUN dotnet publish src/DotnetCleanArch.Api/DotnetCleanArch.Api.csproj \
      -c Release -o /app/publish --no-restore

  # ── Runtime stage ─────────────────────────────────────
  FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
  WORKDIR /app
  RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
  COPY --from=build /app/publish .
  USER appuser
  EXPOSE 8080
  ENV ASPNETCORE_URLS=http://+:8080
  HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
      CMD curl -f http://localhost:8080/health || exit 1
  ENTRYPOINT ["dotnet", "DotnetCleanArch.Api.dll"]
  ```

- [ ] 7.3 Create `docker-compose.yml` at repo root:
  ```yaml
  version: "3.9"
  services:
    api:
      build: .
      ports: ["8080:8080"]
      environment:
        - POSTGRES_CONNECTION_STRING=Host=postgres;Port=5432;Database=${POSTGRES_DB:-dotnetcleanarch};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD:-postgres}
        - REDIS_CONNECTION_STRING=redis:6379
        - JWT_ISSUER=${JWT_ISSUER:-dotnet-clean-arch}
        - JWT_AUDIENCE=${JWT_AUDIENCE:-dotnet-clean-arch}
        - JWT_SECRET=${JWT_SECRET:-change-me-in-production-use-32-chars}
        - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Production}
      depends_on:
        postgres: { condition: service_healthy }
        redis:    { condition: service_healthy }

    postgres:
      image: postgres:16-alpine
      environment:
        POSTGRES_DB: ${POSTGRES_DB:-dotnetcleanarch}
        POSTGRES_USER: ${POSTGRES_USER:-postgres}
        POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-postgres}
      volumes: [postgres_data:/var/lib/postgresql/data]
      healthcheck:
        test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER:-postgres}"]
        interval: 10s; timeout: 5s; retries: 5

    redis:
      image: redis:7-alpine
      volumes: [redis_data:/data]
      healthcheck:
        test: ["CMD", "redis-cli", "ping"]
        interval: 10s; timeout: 5s; retries: 5

  volumes:
    postgres_data:
    redis_data:
  ```

- [ ] 7.4 Create `.env.example` at repo root:
  ```dotenv
  POSTGRES_DB=dotnetcleanarch
  POSTGRES_USER=postgres
  POSTGRES_PASSWORD=changeme
  REDIS_CONNECTION_STRING=localhost:6379
  JWT_ISSUER=dotnet-clean-arch
  JWT_AUDIENCE=dotnet-clean-arch
  JWT_SECRET=change-me-in-production-use-32-chars!!
  ASPNETCORE_ENVIRONMENT=Development
  ```

---

## Group 8 — CI/CD

- [ ] 8.1 Create `.github/workflows/ci.yml`:
  ```yaml
  name: CI
  on:
    push:
      branches: [main, develop]
    pull_request:
      branches: [main]
  jobs:
    build-and-test:
      runs-on: ubuntu-latest
      steps:
        - uses: actions/checkout@v4
        - uses: actions/setup-dotnet@v4
          with: { dotnet-version: "8.0.x" }
        - run: dotnet restore
        - run: dotnet build --no-restore -c Release
        - run: dotnet format --verify-no-changes
        - run: dotnet test --no-build -c Release
            --collect:"XPlat Code Coverage"
            --results-directory ./TestResults
        - uses: actions/upload-artifact@v4
          if: always()
          with:
            name: coverage
            path: TestResults/**/*.xml
  ```

- [ ] 8.2 Create `.github/dependabot.yml`:
  ```yaml
  version: 2
  updates:
    - package-ecosystem: nuget
      directory: "/"
      schedule: { interval: weekly }
    - package-ecosystem: github-actions
      directory: "/"
      schedule: { interval: weekly }
    - package-ecosystem: docker
      directory: "/"
      schedule: { interval: weekly }
  ```

---

## Group 9 — Documentation

- [ ] 9.1 Create `README.md` at repo root with:
  - Badges: `![CI](https://github.com/<your-username>/dotnet-clean-arch/workflows/CI/badge.svg)`
    + dotnet 8 badge + license badge
  - "Quick Start" section: `git clone`, copy `.env.example`, `docker compose up -d`,
    `dotnet ef database update`, `dotnet run --project src/DotnetCleanArch.Api`
  - ASCII architecture diagram (copied from design.md)
  - Environment variable table (from `.env.example` with descriptions)
  - "Project Structure" section with brief description of each project
  - "Adding a New Feature" guide (steps: domain entity → application CQRS →
    infra config → api controller)

- [ ] 9.2 Create `LICENSE` at repo root (MIT, year 2026, `<your-name>` placeholder).

- [ ] 9.3 Create `CONTRIBUTING.md`:
  - Branch naming: `feat/<name>`, `fix/<name>`, `chore/<name>`
  - PR requirements: one concern per PR, tests required, `dotnet format` clean
  - Reference to `.github/pull_request_template.md`

- [ ] 9.4 Create `.github/pull_request_template.md`:
  ```markdown
  ## Summary
  <!-- What does this PR do? -->

  ## Type of Change
  - [ ] Bug fix
  - [ ] New feature
  - [ ] Refactoring
  - [ ] Documentation

  ## Checklist
  - [ ] `dotnet format --verify-no-changes` passes
  - [ ] Unit tests added/updated
  - [ ] Integration tests added/updated (if applicable)
  - [ ] No new warnings introduced
  ```

---

## Group 10 — Initial EF Core Migration

- [ ] 10.1 Run the EF Core migration from within the repo root (requires Docker
  Postgres to be running OR use `--no-build` approach with in-memory fallback):
  ```
  dotnet ef migrations add InitialCreate \
    --project src/DotnetCleanArch.Infrastructure \
    --startup-project src/DotnetCleanArch.Api \
    --output-dir Persistence/Migrations
  ```
  Commit generated migration files to source.

- [ ] 10.2 Verify migration snapshot (`ApplicationDbContextModelSnapshot.cs`) was
  created in `src/DotnetCleanArch.Infrastructure/Persistence/Migrations/`.

- [ ] 10.3 Remove `.gitkeep` from `Persistence/Migrations/` now that real files exist.

---

## Group 11 — Verify Build and Tests Locally

- [ ] 11.1 Run `dotnet build DotnetCleanArch.sln -c Release` — must succeed with 0
  errors and 0 warnings (TreatWarningsAsErrors=true).

- [ ] 11.2 Run `dotnet format --verify-no-changes` — must exit 0.

- [ ] 11.3 Run `dotnet test --configuration Release` from repo root — all unit tests
  must pass. Integration tests can be skipped if Docker is unavailable (add
  `[Trait("Category","Integration")]` and filter via `--filter "Category!=Integration"`).

- [ ] 11.4 Run `docker compose up -d postgres redis` then run full test suite
  including integration tests.

- [ ] 11.5 Run `docker compose up --build` and verify `/health` returns `Healthy`
  at `http://localhost:8080/health`.

- [ ] 11.6 Verify Swagger UI at `http://localhost:8080/swagger` in Development
  environment.

- [ ] 11.7 `git init && git add -A && git commit -m "chore: initial template scaffold"`
  at repo root.

---

## Group 12 — Claude Code Skill

- [ ] 12.1 Create directory `C:\Users\admin\.claude\skills\dotnet-clean-arch\`.

- [ ] 12.2 Create `C:\Users\admin\.claude\skills\dotnet-clean-arch\SKILL.md` with
  the following content:

```markdown
---
name: dotnet-clean-arch
description: >
  Bootstrap a new .NET 8 Clean Architecture project from the DotnetCleanArch
  template. Use when the user asks to start a new .NET project, create a .NET
  Clean Architecture scaffold, or mentions /dotnet-clean-arch.
triggers:
  - /dotnet-clean-arch
  - "new .NET project"
  - "clean architecture scaffold"
  - "dotnet clean arch"
---

# Skill: dotnet-clean-arch

Bootstrap a production-ready .NET 8 Clean Architecture project by renaming the
`DotnetCleanArch` template to a new project name.

## Arguments (prompt if missing)

| Argument | Description | Example |
|---|---|---|
| `TARGET_DIR` | Absolute path for the new project | `D:\Projects\MyService` |
| `NEW_NAME` | PascalCase project name (replaces `DotnetCleanArch`) | `OrderService` |
| `GITHUB_OWNER` | GitHub username or org (for badge/clone URLs) | `acme-corp` |

## Steps

### Step 0 — Gather arguments

If any argument is missing, ask the user before proceeding. Do not proceed with
placeholder values.

Compute derived values:
- `NEW_NAME_KEBAB` = `NEW_NAME` converted to lowercase-kebab-case
  (e.g. `OrderService` → `order-service`)

### Step 1 — Obtain the template

**Option A — Clone from GitHub (preferred once pushed):**
```
git clone https://github.com/<GITHUB_OWNER>/dotnet-clean-arch.git <TARGET_DIR>
```

**Option B — Local copy fallback** (if GitHub not available):
Use the Read + Write tools to copy all files recursively from
`D:\Projects\dotnet-starter\` to `<TARGET_DIR>\`, preserving directory structure.
Skip the `.git\` directory. Skip `openspec\` directory.

After copy: verify `<TARGET_DIR>\DotnetCleanArch.sln` exists.

### Step 2 — Delete existing .git

Delete `<TARGET_DIR>\.git\` directory entirely (use Bash: `Remove-Item -Recurse -Force <TARGET_DIR>\.git`).

### Step 3 — Rename files and folders

Walk `<TARGET_DIR>` depth-first (deepest first). For every file or directory whose
name contains `DotnetCleanArch`, rename it by replacing `DotnetCleanArch` with
`NEW_NAME`.

Use PowerShell pattern (via Bash tool):
```powershell
Get-ChildItem -Path "<TARGET_DIR>" -Recurse -Filter "*DotnetCleanArch*" |
  Sort-Object { $_.FullName.Length } -Descending |
  ForEach-Object {
    $newName = $_.Name -replace 'DotnetCleanArch', '<NEW_NAME>'
    Rename-Item -Path $_.FullName -NewName $newName
  }
```

### Step 4 — Replace content in all text files

For each text file under `<TARGET_DIR>` (extensions: `.cs`, `.csproj`, `.sln`,
`.md`, `.yml`, `.yaml`, `.json`, `.props`, `.targets`, `.env*`, `.editorconfig`,
`.gitignore`, `.dockerignore`, `Dockerfile`):

Apply these replacements **in this order**:

1. `DotnetCleanArch` → `<NEW_NAME>`
2. `dotnet-clean-arch` → `<NEW_NAME_KEBAB>`
3. `<your-username>/dotnet-clean-arch` → `<GITHUB_OWNER>/<NEW_NAME_KEBAB>`

Use the Read tool to read each file, then Write tool to write it back.

For `.sln` files, additionally fix the project GUIDs to be newly generated (use
`[System.Guid]::NewGuid()` in PowerShell or leave as-is — GUIDs in .sln are
cosmetic).

### Step 5 — Re-initialise git

```powershell
Set-Location <TARGET_DIR>
git init
git add -A
git commit -m "chore: scaffold from dotnet-clean-arch template"
```
(Use the Bash tool.)

### Step 6 — Print next steps

Output the following to the user:

```
✅ Project scaffolded at <TARGET_DIR>

Next steps:
  1. cd <TARGET_DIR>
  2. Copy .env.example to .env and fill in secrets:
       cp .env.example .env
  3. Start dependencies:
       docker compose up -d postgres redis
  4. Apply database migrations:
       dotnet ef database update \
         --project src/<NEW_NAME>.Infrastructure \
         --startup-project src/<NEW_NAME>.Api
  5. Run the API:
       dotnet run --project src/<NEW_NAME>.Api
  6. Open Swagger UI:
       http://localhost:5000/swagger  (or the port in launchSettings.json)
  7. Push to GitHub:
       git remote add origin https://github.com/<GITHUB_OWNER>/<NEW_NAME_KEBAB>.git
       git push -u origin main
```
```

- [ ] 12.3 Verify `SKILL.md` is readable and the YAML frontmatter parses correctly
  (check indentation — YAML is whitespace-sensitive).
