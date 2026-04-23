# DotnetCleanArch

![CI](https://github.com/quangnx99/dotnet-clean-arch/workflows/CI/badge.svg)
![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet)
![License](https://img.shields.io/badge/License-MIT-green)

Production-ready .NET 8 Clean Architecture template. No ceremony, no AutoMapper, no MediatR licence risk — just a fast, auditable, AOT-friendly foundation for backend services.

---

## Architecture

```
Domain  <--  Application  <--  Infrastructure  <--  API
  ^               ^
UnitTests     IntegTests (WebApplicationFactory + Testcontainers)
```

```
+--------------------------------------------------------------+
|                          API Layer                           |
|  Controllers -> ISender (Mediator)                           |
|  Program.cs  |  Middleware  |  Swagger  |  HealthChecks      |
+---------------------------+----------------------------------+
                            |
          +-----------------+------------------+
          |                                    |
          v                                    v
+--------------------+          +----------------------------+
|  Application Layer |          |    Infrastructure Layer    |
|                    |          |                            |
|  Commands/Queries  |          |  ApplicationDbContext      |
|  ValidationBehav.  |          |  (EF Core + Npgsql)        |
|  LoggingBehavior   |          |                            |
|  ICacheService ----+--------> |  CacheService (Redis)      |
|  IAppDbContext ----+--------> |  ApplicationDbContext      |
|                    |          |  JwtOptions / AddJwtAuth   |
+--------+-----------+          +----------------------------+
         |
         v
+--------------------+
|    Domain Layer    |
|                    |
|  Result<T>/Error   |
|  Entity base       |
|  AggregateRoot     |
|  IDomainEvent      |
|  Product aggregate |
|  ProductId (VO)    |
|  ProductErrors     |
+--------------------+
```

---

## Quick Start

```bash
# 1. Clone and configure
git clone https://github.com/quangnx99/dotnet-clean-arch.git
cp .env.example .env          # Fill in secrets

# 2. Start dependencies
docker compose up -d postgres redis

# 3. Run the API
dotnet run --project src/DotnetCleanArch.Api
# Swagger: http://localhost:5000/swagger
```

---

## Environment Variables

Configuration is bound to typed options via `Postgres:*` / `Redis:*` / `Jwt:*` keys.
Use env vars with the `__` separator (e.g. `Postgres__Host`) or define them in `.env` and let docker-compose expand.

| Variable | Description | Default |
|---|---|---|
| `POSTGRES_HOST` | PostgreSQL host | `localhost` |
| `POSTGRES_PORT` | PostgreSQL port | `5432` |
| `POSTGRES_DB` | Database name | `dotnetcleanarch` |
| `POSTGRES_USER` | Username | `postgres` |
| `POSTGRES_PASSWORD` | Password | _(required)_ |
| `REDIS_HOST` | Redis host | `localhost` |
| `REDIS_PORT` | Redis port | `6379` |
| `REDIS_PASSWORD` | Redis password (optional) | _(empty)_ |
| `JWT_ISSUER` | JWT token issuer | `dotnet-clean-arch` |
| `JWT_AUDIENCE` | JWT token audience | `dotnet-clean-arch` |
| `JWT_SECRET` | JWT signing key (32+ chars) | _(required)_ |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |

---

## Project Structure

| Project | Purpose |
|---|---|
| `DotnetCleanArch.Domain` | Entities, value objects, domain errors, Result types. Zero NuGet deps. |
| `DotnetCleanArch.Application` | CQRS handlers, validation, pipeline behaviors, interfaces. |
| `DotnetCleanArch.Infrastructure` | EF Core + Npgsql, Redis cache, JWT config, interceptors. |
| `DotnetCleanArch.Api` | Controllers, Swagger, health checks, exception handler. |
| `DotnetCleanArch.UnitTests` | xUnit + FluentAssertions + NSubstitute unit tests. |
| `DotnetCleanArch.IntegrationTests` | Testcontainers-backed end-to-end tests. |

---

## Adding a New Feature

1. **Domain** — Add entity/value object in `src/DotnetCleanArch.Domain/<Feature>/`
2. **Application** — Add Command or Query + Handler + Validator in `src/DotnetCleanArch.Application/<Feature>/`
3. **Infrastructure** — Add EF entity configuration in `src/DotnetCleanArch.Infrastructure/Persistence/Configurations/`
4. **API** — Add controller endpoints in `src/DotnetCleanArch.Api/Controllers/`
5. **Tests** — Add unit tests and integration tests

---

## Key Technology Choices

- **Mediator** (martinothamar) — MIT, source-generated CQRS, zero reflection, AOT-safe
- **FluentValidation** — validation pipeline via `ValidationBehavior`
- **EF Core 8 + Npgsql** — PostgreSQL with snake_case conventions
- **Swashbuckle** — Swagger UI with JWT bearer auth
- **Serilog** — structured logging with Seq sink in development
- **Testcontainers** — real Postgres + Redis in integration tests

---

## License

MIT License. See [LICENSE](LICENSE).
