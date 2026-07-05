---
inclusion: always
---

# Project Overview

## Solution: InfraStudies

A .NET 10 Web API solution structured around vertical slices (features) with a custom in-house Mediator and a shared infrastructure layer.

### Projects

| Project | Type | Purpose |
|---|---|---|
| `IS.Customers.API` | Web API | Customer management API |
| `IS.Core` | Class Library | Shared infrastructure (mediator, domain objects, caching, data utilities, extensions) |
| `IS.Core.API` | Class Library | Shared ASP.NET helpers (base controller, problem details, exception handler) |

### Technology Stack

- **Runtime:** .NET 10
- **Database:** PostgreSQL via Npgsql + EF Core 10
- **Caching:** Redis via `IDistributedCache` / `StackExchange.Redis`
- **Logging:** Serilog (console sink + PostgreSQL sink)
- **Validation:** FluentValidation 12
- **Mediator:** Custom in-house implementation (`IS.Core.Mediator`)
- **API Docs:** Swashbuckle + `Microsoft.AspNetCore.OpenApi` (Swagger UI in Development only)

### Key Conventions at a Glance

- One controller class per endpoint (vertical slice), each extending `BaseController`
- CQRS via `ICommand<TResponse>` / `IQuery<TResponse>` + `IRequestHandler<TRequest, TResponse>`
- All handlers return `BaseResult<T>`; query-only operations may return nullable DTOs directly
- FluentValidation injected directly into each handler — no pipeline behaviors
- EF Core `DbContext` used directly in handlers — no repository or Unit-of-Work abstraction
- Soft deletes via `AuditedEntity.Delete()` (sets `DateDeleted`); global query filter excludes deleted records
- String search columns stored normalized (`NormalizeToUpper`) with GIN trigram indexes
- Cache-aside pattern: GetById handlers check Redis before hitting the database; write handlers invalidate cache
- Migrations run automatically on startup via `PostgresMigrationService` (`BackgroundService`)
