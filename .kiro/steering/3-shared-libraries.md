---
inclusion: always
---

# Shared Libraries Reference

These libraries (`IS.Core` and `IS.Core.API`) are internal shared infrastructure. Do not duplicate anything they already provide.

## IS.Core

### Domain Objects (`IS.Core.DomainObjects`)

| Type | Purpose |
|---|---|
| `BaseEntity` | Abstract base with `Guid Id` (generated in constructor), value equality by `Id` |
| `AuditedEntity : BaseEntity` | Adds `DateCreated` (UTC, set in constructor), `DateDeleted` (nullable), `Delete()` method |
| `IAggregateRoot` | Marker interface; apply to top-level aggregate entities |

### Mediator (`IS.Core.Mediator`)

| Type | Purpose |
|---|---|
| `IRequest<TResponse>` | Base request interface |
| `ICommand<TResponse>` | Marker for commands (extends `IRequest`) |
| `IQuery<TResponse>` | Marker for queries (extends `IRequest`) |
| `IRequestHandler<TRequest, TResponse>` | Handler interface — implement `HandleAsync(request, ct)` |
| `IMediator` | Dispatcher — call `SendAsync(request, ct)` from endpoints |

Registration: `services.AddMediator(typeof(SomeCommand).Assembly)`

### Communication (`IS.Core.Communication`)

| Type | Purpose |
|---|---|
| `BaseResult<T>` | Result wrapper with `Response`, `Errors`, `IsValid` |
| `BaseResult<T>.Success(value)` | Factory for success results |
| `BaseResult<T>.Failure(string)` | Factory with a single "General" error |
| `BaseResult<T>.Failure(dictionary)` | Factory from FluentValidation's `ToDictionary()` |
| `AddError(key, message)` | Add a named error to an existing result |
| `AddGeneralError(message)` | Add an error under the "General" key |

### Cache (`IS.Core.Cache`)

| Type | Purpose |
|---|---|
| `ICacheService` | Get/Set/Remove via Redis |
| `GetAsync<T>(key, ct)` | Returns `T?` (null on cache miss) |
| `SetAsync<T>(key, data, options, ct)` | Serializes and stores |
| `RemoveAsync(key, ct)` | Evicts a key |

Registration: `services.AddCache(configuration, "RedisConnection", "IS.SomeApi")`

Typical expiration: `new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15))`

### Data (`IS.Core.Data`)

| Extension/Type | Purpose |
|---|---|
| `ModelBuilderExtensions.SetDefaultConfiguration(assembly)` | Enables `pg_trgm`, applies all `IEntityTypeConfiguration` classes from the given assembly |
| `ModelConfigurationBuilder.SetDefaultConfigurationConventions()` | Sets global column type defaults (string → VARCHAR 255, DateTime → timestamptz) |
| `ToPagedResultAsync(pageNumber, pageSize, ct)` | EF Core IQueryable extension returning `PagedResult<T>` |
| `PaginationParameters` | Base record with `PageNumber` and `PageSize` |
| `PagedResult<T>` | Record with `Data`, `PageNumber`, `PageSize`, `TotalCount`, `TotalPages`, `HasNext` |

### Extensions (`IS.Core.Extensions`)

| Extension | Purpose |
|---|---|
| `string.NormalizeToUpper()` | Strips diacritics, removes non-alphanumeric characters, collapses whitespace, uppercases — use on all text fields that require search/filtering |

### Logging (`IS.Core.Logging`)

Registration: `builder.AddDefaultSerilog("LogsConnection")` (configures Serilog with console + PostgreSQL sinks)

---

## IS.Core.API

### Controllers (`IS.Core.API.Controllers`)

| Type | Purpose |
|---|---|
| `BaseController : ControllerBase` | Base for all endpoint controllers; provides `BadRequestProblem(errors)` helper |

All endpoint controllers must extend `BaseController` and be decorated with `[ApiController]` (inherited).

### Results (`IS.Core.API.Results`)

| Type | Purpose |
|---|---|
| `BadRequestProblemDetails : ProblemDetails` | Standard 400 response body with `errors` dictionary and `traceId`; used in `[ProducesResponseType]` attributes |

### Exceptions (`IS.Core.API.Exceptions`)

Registration: `services.AddGlobalExceptionHandler()` + `app.UseExceptionHandler()`

Catches all unhandled exceptions and returns a `500 ProblemDetails` response. No try/catch blocks needed in application code.

---

## Configuration Extension Methods (IS.Customers.API conventions)

These patterns should be replicated in any new API project:

```csharp
// Program.cs
builder.Services
    .AddOpenApi()
    .AddGlobalExceptionHandler()
    .AddDbContext<MyDbContext>("PostgresConnection", builder.Environment)
    .AddDependenciesConfiguration()           // local validators + background services
    .AddMediator(typeof(SomeCommand).Assembly)
    .AddCache(builder.Configuration, "RedisConnection", "IS.My.API")
    .AddControllers();

builder.AddDefaultSerilog("LogsConnection");
```
