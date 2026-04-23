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

Postgres uses the standard `ConnectionStrings:Default` key.
Redis is bound to a typed `Redis` section so individual fields (host/port/password/ssl) can be overridden separately.

| Variable | Description | Default |
|---|---|---|
| `ConnectionStrings__Default` | Postgres connection string | `Host=localhost;Port=5432;...` |
| `Redis__Host` | Redis host | `localhost` |
| `Redis__Port` | Redis port | `6379` |
| `Redis__Password` | Redis password (optional) | _(empty)_ |
| `Redis__Ssl` | Use TLS | `false` |
| `Jwt__Issuer` | JWT token issuer | `dotnet-clean-arch` |
| `Jwt__Audience` | JWT token audience | `dotnet-clean-arch` |
| `Jwt__SecretKey` | JWT signing key (32+ chars) | _(required)_ |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |

---

## Bootstrap a new project via Claude Code skill

This repo ships a ready-to-install Claude Code skill that clones + renames
the template into a fresh project in one command.

**Install (user scope, once per machine):**

```powershell
# Windows
New-Item -ItemType Directory -Force -Path "$env:USERPROFILE\.claude\skills\dotnet-clean-arch" | Out-Null
Copy-Item ".\.claude\skills\dotnet-clean-arch\SKILL.md" `
          "$env:USERPROFILE\.claude\skills\dotnet-clean-arch\SKILL.md" -Force
```

```bash
# macOS / Linux
mkdir -p ~/.claude/skills/dotnet-clean-arch
cp .claude/skills/dotnet-clean-arch/SKILL.md ~/.claude/skills/dotnet-clean-arch/SKILL.md
```

**Use it:**
```
/dotnet-clean-arch
```
Claude will ask for `TARGET_DIR`, `NEW_NAME`, `GITHUB_OWNER`, then clone,
rename every `DotnetCleanArch` → your name, reset git history, and print
next steps.

See [`.claude/skills/dotnet-clean-arch/README.md`](.claude/skills/dotnet-clean-arch/README.md)
for full details, offline install, and uninstall.

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
