# InfraStudies Agent Guide

## Project Overview

- Solution: `InfraStudies`
- Stack: .NET 10 Web API, PostgreSQL, EF Core 10, Redis, Serilog, FluentValidation 12, Swagger/OpenAPI
- Architecture: vertical slice features, CQRS, custom in-house mediator, shared infrastructure libraries

## Solution Structure

- `IS.Customers.API`: main Web API project
- `IS.Core`: shared infrastructure for mediator, domain objects, cache, data utilities, extensions, and logging
- `IS.Core.API`: shared ASP.NET helpers such as `BaseController`, `ProblemDetails`, and exception handling

## Core Architecture Rules

- Use vertical slices under `Features/<FeatureName>/`
- Keep one controller class per endpoint and one action per controller
- Use CQRS:
  - commands implement `ICommand<TResponse>`
  - queries implement `IQuery<TResponse>`
  - handlers implement `IRequestHandler<TRequest, TResponse>`
- Use the custom mediator via `IMediator.SendAsync(request, cancellationToken)`
- Do not add extra controller layers, repository abstractions, or Unit of Work wrappers

## Shared Libraries

### `IS.Core`

- `BaseEntity`: base entity with `Guid Id` and equality by `Id`
- `AuditedEntity`: adds UTC `DateCreated`, nullable `DateDeleted`, and `Delete()`
- `IAggregateRoot`: marker interface for aggregate roots
- `BaseResult<T>`: standard result wrapper with `Success` and `Failure` factories
- `ICacheService`: Redis cache access with `GetAsync`, `SetAsync`, and `RemoveAsync`
- `PaginationParameters` and `PagedResult<T>`: pagination primitives
- `NormalizeToUpper()`: normalize searchable text fields

### `IS.Core.API`

- `BaseController`: base class for endpoints and helper for `BadRequestProblem(errors)`
- `BadRequestProblemDetails`: standard 400 payload
- Global exception handler: use `AddGlobalExceptionHandler()` and `UseExceptionHandler()`

## Domain Rules

- Put domain behavior on entities, not in handlers
- Use `AuditedEntity.Delete()` for soft deletes
- Never hard-delete entities that inherit from `AuditedEntity`
- Aggregate roots should implement `IAggregateRoot`

## Command and Query Rules

- Prefer `record` when practical for commands, queries, and DTOs
- Commands should use `init` for request-body properties
- Use `set` only for properties populated outside the body; apply `[JsonIgnore]` to those properties when needed
- Queries that extend `PaginationParameters` should be `record`
- Queries that are plain DTO-like requests may use `class` or `record` depending on clarity
- Commands return `BaseResult<T>`
- Queries may return `BaseResult<T>` or nullable DTOs directly when appropriate

## Validation Rules

- Create one `AbstractValidator<T>` per command or query that needs validation
- Inject validators directly into handlers
- First step in a handler: validate the request
- Return `BaseResult<T>.Failure(validationResult.ToDictionary())` for invalid input
- Use `Cascade(CascadeMode.Stop)` when chaining constraints
- Add clear `.WithName(...)` and `.WithMessage(...)` text when property names are not user-friendly
- Inject `DbContext` directly into validators when database checks are required

## Handler Rules

- Keep handlers thin
- Load the entity, call entity behavior, persist changes, then invalidate cache when needed
- Use `AsNoTracking()` for read-only queries
- Pass `CancellationToken` through every async operation
- Avoid throwing exceptions for expected business failures
- Use `BaseResult<T>.Failure("...")` for business-rule failures

## Endpoint Rules

- Every endpoint controller must inherit from `BaseController`
- Decorate controllers with `[ApiController]`, `[Route("api/<resource>")]`, and `[Tags("<Group>")]`
- Use `[FromBody]` for commands and `[FromQuery]` for queries
- Use `[FromRoute]` for route parameters
- Declare `[ProducesResponseType]` for every possible status code
- Return:
  - `201 Created` for successful create operations
  - `200 OK` for successful queries and deletes
  - `400 Bad Request` for validation failures
  - `404 Not Found` when an entity is missing
- Include XML documentation and a sample body in `<remarks>` when the endpoint accepts a body

## Data Access Rules

- Use `DbContext` directly inside handlers
- Apply global EF Core conventions from `IS.Core`
- Do not edit migration files manually
- Generate migrations with `dotnet ef migrations add <Name>`
- Use `ModelBuilderExtensions.SetDefaultConfiguration(assembly)` to apply mappings and enable `pg_trgm`
- Use `ModelConfigurationBuilder.SetDefaultConfigurationConventions()` for global type defaults
- Add `HasMaxLength` only when a value differs from the global default
- Define indexes in `IEntityTypeConfiguration<T>` classes

## Search and Normalization

- Store searchable text in both raw and normalized columns when needed
- Normalize values at entity construction time with `NormalizeToUpper()`
- Apply `NormalizeToUpper()` to filter values at query time
- Use `EF.Functions.ILike` against normalized columns
- Add both a B-tree index and a GIN trigram index to normalized searchable columns

## Cache Rules

- Use cache-aside for read-heavy point lookups such as `GetById`
- Check Redis first with `ICacheService.GetAsync<T>(key, ct)`
- On cache miss, query the database and cache the result when not null
- Use a 15-minute absolute expiration as the default
- Invalidate cache immediately after successful write or delete operations
- Keep cache key constants in `Shared/CacheKeys.cs`

## Pagination Rules

- Use `PaginationParameters` for paged queries
- Return `BaseResult<PagedResult<TDto>>`
- Use `ToPagedResultAsync(pageNumber, pageSize, ct)` instead of manual paging logic

## Dependency Injection Rules

- Register validators with `AddValidatorsFromAssemblyContaining<TAnyValidator>()`
- Register handlers with `AddMediator(typeof(SomeCommand).Assembly)`
- Register `DbContext` with the project-specific `AddDbContext<TContext>(connectionStringId, env)` extension
- Register cache with `AddCache(configuration, "RedisConnection", "IS.SomeApi")`
- Register migrations service as `AddHostedService`

## Feature Creation Flow

1. Create the feature folder under `Features/<ActionAggregate>/`
2. Define the command or query
3. Add a DTO when the query returns structured data
4. Add a validator when input needs validation
5. Implement the handler
6. Implement the endpoint
7. Add a migration if the schema changed
8. Add normalized columns and indexes when introducing searchable text

## Naming Conventions

- Commands: `<Action><Aggregate>Command`
- Queries: `<Action><Aggregate>Query`
- Handlers: `<CommandOrQueryName>Handler`
- Validators: `<CommandOrQueryName>Validator`
- Endpoints: `<Action><Aggregate>Endpoint`
- DTOs: `<Aggregate>DTO` or `<Context><Aggregate>DTO`
- EF mappings: `<Entity>Mapping`
- Config classes: `<Concern>Config`
- Feature folders: PascalCase and action-first
- Cache keys: `CacheKeys` static class in `Shared/`

## Logging and Errors

- Use `ILogger<T>` for structured logging
- Prefer business-result failures over exceptions for expected cases
- Let the global exception handler produce the 500 response for unhandled errors

## Commit Rules

- Never commit automatically
- Wait for explicit user approval before committing or pushing
- Use Conventional Commits format:
  - `<type>(<scope>): <subject>`
  - allowed types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`
- Keep the subject line under 50 characters
- Use imperative mood
- Include a body with bullet points for non-trivial changes

## Communication Rules

- Default language for conversation: `pt-BR`
- Code and technical specs should stay in English
- Be direct, concise, and technically clear
- Structure responses with a main conclusion first, then details, then trade-offs when relevant, and a clear next step
- Avoid vague, generic, or unactionable replies

## File Format Rules

- Keep source and documentation files in LF line endings
- Do not create LF-only files
- Keep Markdown simple and practical for agent consumption
