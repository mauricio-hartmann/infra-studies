---
inclusion: always
---

# Architecture & Patterns

## Vertical Slice Architecture

Each feature lives in its own folder under `Features/`. A complete slice contains:

```
Features/
  CreateCustomer/
    CreateCustomerCommand.cs          // ICommand<BaseResult<Guid>>
    CreateCustomerCommandHandler.cs   // IRequestHandler<...>
    CreateCustomerCommandValidator.cs // AbstractValidator<CreateCustomerCommand>
    CreateCustomerEndpoint.cs         // Controller (one action method)
```

For queries, replace `Command` with `Query` throughout. Add a `*DTO.cs` file for the query response shape.

There is no shared "controllers" folder. Every endpoint is its own controller class.

## CQRS

- **Commands** implement `ICommand<TResponse>` (mutate state, always return `BaseResult<T>`)
- **Queries** implement `IQuery<TResponse>` (read-only, return `BaseResult<T>` or a nullable DTO)
- The commands use `init` properties when all properties come from the request body. Properties that do not come from the request body can use `set`. Queries use `record` when they extend `PaginationParameters`, otherwise use `class`.
- When possible, use `record` for the type of commands and queries.
- The custom `IMediator.SendAsync(request, cancellationToken)` dispatches to the matching handler.

## Mediator Handler Discovery

Handlers are auto-discovered by reflection at startup (`AddMediator(assembly)`). The mediator validates at startup that:
1. No two handlers exist for the same request type (throws `InvalidOperationException`)
2. Every `IRequest<T>` in the assembly has a handler (throws `InvalidOperationException`)

**One handler per request type is enforced.**

## Domain Objects

Hierarchy (all in `IS.Core.DomainObjects`):

```
BaseEntity          → Guid Id, equality by Id
  └── AuditedEntity → DateCreated (UTC), DateDeleted (nullable), Delete() method
        └── (domain entities, e.g. Customer, Address)
```

Aggregate roots implement `IAggregateRoot` (marker interface). Domain behavior belongs on the entity itself (e.g., `Customer.AddAddress`, `Customer.DeleteAddress`).

## Soft Deletes

- Call `entity.Delete()` to set `DateDeleted = DateTime.UtcNow`
- A global EF Core query filter excludes records where `DateDeleted != null`
- Never hard-delete entities that extend `AuditedEntity`

## Result Pattern

Every command handler returns `BaseResult<T>`:

```csharp
// Success
return BaseResult<Guid>.Success(entity.Id);

// Validation failure (from FluentValidation)
return BaseResult<T>.Failure(validationResult.ToDictionary());

// Business rule failure
return BaseResult<T>.Failure("Customer does not exist!");
```

Endpoints map the result to HTTP responses:
```csharp
return result.IsValid ? Created(...) : BadRequestProblem(result.Errors);
```

Query handlers that return a nullable DTO directly (no `BaseResult` wrapper) use `NotFound` when the result is null.

## Cache-Aside Pattern

Applied to read-heavy, point-lookup queries (`GetById`):

1. Check Redis with `ICacheService.GetAsync<T>(key, ct)`
2. On cache miss: query the database
3. Store result in Redis with `ICacheService.SetAsync(key, data, options, ct)` (15-minute absolute expiration as default) when result is not null.

Write/delete handlers must invalidate the cache with `ICacheService.RemoveAsync(key, ct)` immediately after saving when is updating/deleting some entity.

Cache key constants live in a `CacheKeys` static class within the API project's `Shared/` folder.

## Pagination

Paged queries use `PaginationParameters` as the base record and return `BaseResult<PagedResult<TDto>>`.  
Call the `.ToPagedResultAsync(pageNumber, pageSize, ct)` EF Core extension instead of manual `Skip`/`Take`.

## EF Core Conventions (applied globally in `IS.Core`)

- All `string` properties default to `VARCHAR(255)`
- All `DateTime` / `DateTime?` properties map to `timestamp with time zone`
- `pg_trgm` extension is enabled on every `DbContext` via `SetDefaultConfiguration`
- Search-optimized text columns must have both a B-tree index and a GIN trigram index (`gin_trgm_ops`)

## Dependency Injection Registration

- Validators: `AddValidatorsFromAssemblyContaining<TAnyValidator>()` in `DependencyInjectionConfig`
- Handlers: auto-registered by `AddMediator(typeof(SomeCommand).Assembly)` in `Program.cs`
- DbContext: `AddDbContext<TContext>(connectionStringId, env)` extension (retry on failure, sensitive data logging in Dev)
- Cache: `AddCache(configuration, "RedisConnection", "IS.SomeApi")` extension
- Migrations: `PostgresMigrationService` registered as `AddHostedService`
