---
inclusion: always
---

# Adding a New Feature (Step-by-Step)

Follow this checklist whenever adding a new endpoint to an existing API project.

## 1. Create the Feature Folder

```
Features/<ActionAggregate>/
```

Example: `Features/UpdateCustomer/`

## 2. Define the Command or Query

**Command** (mutates state):
```csharp
// UpdateCustomerCommand.cs
public record UpdateCustomerCommand : ICommand<BaseResult<bool>>
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public string TradeName { get; init; }
    // ...
}
```

**Query** (read-only):
```csharp
// GetCustomerByIdQuery.cs
public record GetCustomerByIdQuery : IQuery<CustomerDTO>
{
    public Guid Id { get; init; }
}
```

## 3. Define the DTO (when a DTO should be returned)

Use `record` for DTOs:
```csharp
public record CustomerDTO
{
    public Guid Id { get; init; }
    public string LegalName { get; init; }
    // ...
}
```

## 4. Write the Validator (if needed)

```csharp
public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator(CustomerDbContext db)
    {
        RuleFor(x => x.Id).NotEmpty().WithName("Id").WithMessage("{PropertyName} is required!");
        RuleFor(x => x.TradeName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithName("Trade name").WithMessage("{PropertyName} is required!")
            .MaximumLength(255).WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");
    }
}
```

Validators are auto-registered via `AddValidatorsFromAssemblyContaining` — no manual DI registration needed.

## 5. Write the Handler

```csharp
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, BaseResult<bool>>
{
    private readonly UpdateCustomerCommandValidator _validator;
    private readonly CustomerDbContext _customerDbContext;
    private readonly ICacheService _cacheService;

    public UpdateCustomerCommandHandler(UpdateCustomerCommandValidator validator,
                                        CustomerDbContext customerDbContext,
                                        ICacheService cacheService)
    {
        _validator = validator;
        _customerDbContext = customerDbContext;
        _cacheService = cacheService;
    }

    public async Task<BaseResult<bool>> HandleAsync(UpdateCustomerCommand request, CancellationToken cancellationToken = default)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BaseResult<bool>.Failure(validationResult.ToDictionary());

        Customer? customer = await _customerDbContext.Customers
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (customer is null)
            return BaseResult<bool>.Failure("Customer does not exist!");

        // delegate behavior to entity
        customer.UpdateTradeName(request.TradeName);

        await _customerDbContext.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync(CacheKeys.Customer(customer.Id), cancellationToken);

        return BaseResult<bool>.Success(true);
    }
}
```

Handlers are auto-discovered by `AddMediator` — no manual DI registration needed.

## 6. Write the Endpoint

```csharp
[Route("api/customer")]
[Tags("Customers")]
public class UpdateCustomerEndpoint : BaseController
{
    private readonly IMediator _mediator;

    public UpdateCustomerEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    //// <summary>
    /// Ipdate a customer.
    /// </summary>
    /// <param name="product">The customer to update.</param>
    /// <returns>Returns the operation status.</returns>
    /// <remarks>
    /// Sample request body:
    ///
    /// {
    ///   "tradeName": "New Trade Name",
    /// }
    /// </remarks>
    /// <response code="200">Returns the operation status</response>
    /// <response code="400">If operation fails</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCustomerAsync([FromRoute] Guid id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        // bind route param into command if needed via a mapping step
        BaseResult<Guid> result = await _mediator.SendAsync(command, cancellationToken);

        return result.IsValid ? Ok() : BadRequestProblem(result.Errors);
    }
}
```

## 7. Run the Migration (if the schema changed)

```bash
dotnet ef migrations add <MigrationName> --project src/IS.Customers.API
```

Never edit migration files manually.

## Checklist Summary

- [ ] Feature folder created
- [ ] Command or Query defined (correct interface: `ICommand` / `IQuery`)
- [ ] DTO defined
- [ ] Validator written (if input needs validation)
- [ ] Handler written (validate → business logic → persist → cache invalidate → return `BaseResult`)
- [ ] Endpoint written (extends `BaseController`, one action, all `[ProducesResponseType]` attributes present)
- [ ] Migration added (if schema changed)
- [ ] Cache invalidated on write operations
- [ ] Normalized column + index added if a new searchable text field was introduced
