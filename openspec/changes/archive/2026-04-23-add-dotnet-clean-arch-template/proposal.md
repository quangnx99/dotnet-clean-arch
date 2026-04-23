# Proposal: add-dotnet-clean-arch-template

## Problem Statement

Starting a new .NET backend project from scratch requires manually wiring together
Clean Architecture layers, CQRS plumbing, EF Core with Postgres, Redis caching,
JWT auth scaffolding, observability, Docker support, and CI/CD. This repeated
ceremony adds hours of setup friction before any business logic can be written.

## Proposed Solution

Produce a production-ready, opinionated .NET 8 Clean Architecture template at
`D:\Projects\dotnet-starter` that can be pushed to GitHub and reused via a
companion Claude Code skill. Every decision in the template is locked to eliminate
debate on new projects.

## Deliverable Groups

### 1. Repo-root Infrastructure Files
`.gitignore`, `.editorconfig`, `Directory.Build.props`, `Directory.Packages.props`,
`global.json`, `DotnetCleanArch.sln`

### 2. Documentation & Community Files
`README.md`, `LICENSE` (MIT 2026), `CONTRIBUTING.md`,
`.github/pull_request_template.md`

### 3. Domain Layer — `src/DotnetCleanArch.Domain`
Custom `Result<T>` / `Error` / `ErrorType`, `Entity` base, `AggregateRoot`,
`IDomainEvent`, and the sample `Product` aggregate with strongly-typed `ProductId`
and `ProductErrors`.

### 4. Application Layer — `src/DotnetCleanArch.Application`
CQRS messaging abstractions, `ValidationBehavior`, `LoggingBehavior`,
`ICacheService`, `IUnitOfWork`, `IApplicationDbContext`, sample Product CQRS
(CreateProduct command + GetProductById query + manual mapping extension),
`DependencyInjection.cs`.

### 5. Infrastructure Layer — `src/DotnetCleanArch.Infrastructure`
`ApplicationDbContext`, EF entity configurations, auditable-entities interceptor,
`UnitOfWork`, `CacheService` (IDistributedCache / Redis), JWT options scaffold,
`DependencyInjection.cs`.

### 6. API Layer — `src/DotnetCleanArch.Api`
`Program.cs`, `ApiController` base, `ProductsController`, global exception handler
(ProblemDetails), `ResultExtensions`, Swagger/OpenAPI configuration,
`appsettings.json`, `appsettings.Development.json`.

### 7. Unit Tests — `tests/DotnetCleanArch.UnitTests`
xUnit + FluentAssertions + NSubstitute. Tests for `Result`, `Product` domain
logic, `CreateProductHandler`, `GetProductByIdHandler`.

### 8. Integration Tests — `tests/DotnetCleanArch.IntegrationTests`
xUnit + Testcontainers (real Postgres + Redis) + `WebApplicationFactory`. Tests for
Product endpoints end-to-end.

### 9. Docker
`Dockerfile` (multi-stage, non-root, HEALTHCHECK), `docker-compose.yml`
(api + postgres:16-alpine + redis:7-alpine), `.dockerignore`, `.env.example`.

### 10. CI/CD
`.github/workflows/ci.yml` (dotnet restore/build/format-check/test + coverage
artifact), `.github/dependabot.yml` (nuget + github-actions + docker, weekly).

### 11. Initial EF Core Migration
`Persistence/Migrations/` — initial migration committed in source.

### 12. Claude Code Skill
`C:\Users\admin\.claude\skills\dotnet-clean-arch\SKILL.md` — accepts target dir +
new project name + GitHub owner, clones/copies template, renames all occurrences
of `DotnetCleanArch`, re-initialises git, prints next-steps.

## Non-Goals
- No ASP.NET Identity (JWT scaffold only — auth wired by consumer)
- No AutoMapper or Mapster (manual extension methods)
- No MediatR (replaced by `Mediator` by martinothamar — MIT, source-generated)
- No gRPC, no GraphQL, no message bus — CQRS over HTTP only in template
