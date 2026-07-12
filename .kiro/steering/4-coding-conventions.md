---
inclusion: always
---

# Coding Conventions

## Naming

| Artifact        | Convention                                          | Example                                       |
| -----------------| -----------------------------------------------------| -----------------------------------------------|
| Commands        | `<Action><Aggregate>Command`                        | `CreateCustomerCommand`                       |
| Queries         | `<Action><Aggregate>Query`                          | `GetPagedCustomersQuery`                      |
| Handlers        | `<Command/QueryName>Handler`                        | `CreateCustomerCommandHandler`                |
| Validators      | `<Command/QueryName>Validator`                      | `CreateCustomerCommandValidator`              |
| Endpoints       | `<Action><Aggregate>Endpoint`                       | `CreateCustomerEndpoint`                      |
| DTOs            | `<Aggregate>DTO` or `<Context><Aggregate>DTO`       | `CustomerDTO`, `PagedCustomerDTO`             |
| EF Mappings     | `<Entity>Mapping`                                   | `CustomerMapping`                             |
| Config classes  | `<Concern>Config` (static class, extension methods) | `DatabaseConfig`, `DependencyInjectionConfig` |
| Feature folders | PascalCase, action-first                            | `CreateCustomer/`, `GetCustomerById/`         |
| Cache keys      | `CacheKeys` static class in `Shared/`               | `CacheKeys.Customer(id)`                      |

## Folder Layout

```
IS.SomeApi/
  Background/           # BackgroundService implementations
  Configuration/        # Static extension-method config classes
  Data/
    Mapping/            # IEntityTypeConfiguration<T> per entity
    SomeDbContext.cs
  Entities/             # Domain entities (extend AuditedEntity + IAggregateRoot)
  Features/
    <FeatureName>/      # All files for one vertical slice in one folder
  Migrations/           # EF Core generated — never edit manually
  Shared/               # Cross-feature utilities (CacheKeys, constants)
```

## C# Language Rules

- **`init` on command properties** — commands use `init` in properties that comes from request body, when property needs to be seted after command creation (like a URL parameter) use `set`. When using set in a property, add `[JsonIgnore]` from `System.Text.Json.Serialization`.
- **`record` for queries and DTOs** — use `record` for purely data types (queries, DTOs, pagination parameters) and commands whenever possible; use `class` for handlers
- **`== null` / `!= null`** — preferred over `is null` / `is not null`
- **Target-typed `new()`** — use when the type is clear from context (e.g., `Addresses = []`)
- **Implicit usings** — enabled; do not add `using System;` etc. unless required for disambiguation
- **`async`/`await` throughout** — never `.Result` or `.Wait()`; always propagate `CancellationToken` to every async call
- **Use types over var**: when the value to the right of `=` does not make the variable type explicit, use the type. Use `var` only when the value to the right of `=` makes the type clear. Example: `var example = new StringBuilder()`
- **if/else and loop blocks**: always let a blank line after and before if/else blocks and loop blocks (for, while, etc)

## Endpoints

- Extend `BaseController` from `IS.Core.API`; one controller class = one action method
- Decorate with `[Route("api/<resource>")]` and `[Tags("<Group>")]`
- Declare `[ProducesResponseType]` for **every** possible status code
- Accept `CancellationToken` as a parameter and pass it to every async call
- Use `[FromBody]` for commands, `[FromQuery]` for queries
- When the endpoint has a body, create a body example using `<remarks>` endpoint xml documentation
- - Use `[FromRoute]` to indicate when the endpoint method parameter comes from route

**`BaseResult` → HTTP mapping:**

| Scenario           | Response                             |
| --------------------| --------------------------------------|
| Create success     | `201 Created` with `Location` header |
| Query success      | `200 OK`                             |
| Delete success     | `200 OK`                             |
| Validation failure | `BadRequestProblem(result.Errors)`   |
| Entity not found   | `NotFound()`                         |

## Validators

- One `AbstractValidator<T>` per command or query that requires validation
- Apply `Cascade(CascadeMode.Stop)` on rules with multiple chained constraints
- Every rule must have `.WithName("Human readable name")` when the property name is not user friendly and `.WithMessage(...)`
- Inject `DbContext` directly into validators that need database checks
- Validators are auto-registered via `AddValidatorsFromAssemblyContaining` — no manual DI needed

## Handlers

- Inject the validator directly in the constructor (no pipeline behavior)
- First line of `HandleAsync`: call `await _validator.ValidateAsync(request, ct)` (commands) or `_validator.Validate(request)` (synchronous queries) when a validator for the command/query exists
- Return `BaseResult<T>.Failure(validationResult.ToDictionary())` immediately on invalid input
- Keep handlers thin — delegate domain behavior to the entity, not the handler
- Handlers are auto-discovered by `AddMediator` — no manual DI needed

**Canonical handler structure:**

```csharp
public async Task<BaseResult<T>> HandleAsync(MyCommand request, CancellationToken cancellationToken = default)
{
    ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
        return BaseResult<T>.Failure(validationResult.ToDictionary());

    // fetch → mutate entity → persist → invalidate cache
    await _dbContext.SaveChangesAsync(cancellationToken);
    await _cacheService.RemoveAsync(CacheKeys.SomeKey(id), cancellationToken);

    return BaseResult<T>.Success(value);
}
```

## EF Core / Data Access

- Use `DbContext` directly in handlers — no repository or Unit-of-Work wrappers
- Add `AsNoTracking()` on all read-only queries
- Text search: use `EF.Functions.ILike` against the normalized column, and apply `NormalizeToUpper()` to the filter value at query time
- `HasMaxLength` in mappings only when the value differs from the global 255-character default
- Define all indexes inside `IEntityTypeConfiguration<T>` — not on the entity class
- Never edit migration files; generate with `dotnet ef migrations add <Name>`

## String Normalization

Every searchable text field needs both a raw column and a normalized companion:

```csharp
// In the entity constructor
LegalName = legalName;
NormalizedLegalName = legalName.NormalizeToUpper();
```

`NormalizeToUpper()` (from `IS.Core.Extensions`) strips diacritics, removes non-alphanumeric/space characters, collapses whitespace, and uppercases the result. Apply it at entity construction **and** to filter values at query time. Normalized columns must have both a B-tree index and a GIN trigram index (`gin_trgm_ops`).

## Cache-Aside Pattern

Applied to point-lookup (`GetById`) query handlers:

1. `ICacheService.GetAsync<T>(key, ct)` — return immediately on hit
2. On miss: query the database with `AsNoTracking()`
3. Store in Redis when data is not null: `ICacheService.SetAsync(key, data, options, ct)` — 15-minute absolute expiration as default
4. Write/delete handlers must call `ICacheService.RemoveAsync(key, ct)` after `SaveChangesAsync`

Cache key constants live in `Shared/CacheKeys.cs`.

## Error Handling

- Return business errors via `BaseResult<T>.Failure(...)` — never throw exceptions for expected failures
- `GlobalExceptionHandler` (registered via `AddGlobalExceptionHandler()`) handles all unhandled exceptions and returns `500 ProblemDetails` — no try/catch in handlers or endpoints

## Logging

- Inject `ILogger<T>` to log data
- Use structured logging: `_logger.LogInformation("Processing {Action} for {Id}", action, id)`
- Registered via `builder.AddDefaultSerilog("LogsConnection")`

## Line Endings

All source files (`.cs`, `.json`, `.md`, `.csproj`, `.slnx`) must use **LF (`\n`)** line endings. When creating or editing files programmatically, write LF explicitly. Do not commit CRLF-only files.
