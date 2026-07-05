# Implementation Plan: Update Customer

## Overview

Implement the `UpdateCustomer` slice in the `IS.Customers.API` project. The plan covers: domain method on the entity, command, validator, handler, and endpoint. No migration is needed.

## Tasks

- [x] 1. Add `Update` method to the `Customer` entity
  - Open `src/IS.Customers.API/Entities/Customer.cs`
  - Add the public method `Update(string legalName, string tradeName, string? email, string mainPhone, string? secondaryPhone, string? siteUrl, string mainContactName)`
  - Inside the method: assign all fields and recalculate `NormalizedLegalName = legalName.NormalizeToUpper()` and `NormalizedTradeName = tradeName.NormalizeToUpper()`
  - _Requirements: 1.2, 1.4_

- [x] 2. Create `UpdateCustomerCommand`
  - Create folder `src/IS.Customers.API/Features/UpdateCustomer/`
  - Create `UpdateCustomerCommand.cs`
  - `record` implementing `ICommand<BaseResult<bool>>`
  - Property `Id` with `set` and `[JsonIgnore]` (comes from route)
  - Other properties with `init`: `LegalName`, `TradeName`, `Email?`, `MainPhone`, `SecondaryPhone?`, `SiteUrl?`, `MainContactName`
  - _Requirements: 1.3, 4.4_

- [x] 3. Create `UpdateCustomerCommandValidator`
  - Create `UpdateCustomerCommandValidator.cs` in the `Features/UpdateCustomer/` folder
  - Inherit `AbstractValidator<UpdateCustomerCommand>`
  - Do **not** inject `CustomerDbContext` -- no customer existence check in validator
  - Rules (with `Cascade(CascadeMode.Stop)` where there are multiple constraints):
    - `LegalName`: `NotEmpty` (WithName "Legal name") + `MaximumLength(255)`
    - `TradeName`: `NotEmpty` (WithName "Trade name") + `MaximumLength(255)`
    - `MainPhone`: `NotEmpty` (WithName "Main phone") + `MaximumLength(50)`
    - `MainContactName`: `NotEmpty` (WithName "Main contact name") + `MaximumLength(255)`
    - `Email` (when not null/empty): `MaximumLength(100)`
    - `SecondaryPhone` (when not null/empty): `MaximumLength(50)` (WithName "Secondary phone")
    - `SiteUrl` (when not null/empty): `MaximumLength(100)` (WithName "Site URL")
  - _Requirements: 2.1-2.13_

- [x] 4. Create `UpdateCustomerCommandHandler`
  - Create `UpdateCustomerCommandHandler.cs` in the `Features/UpdateCustomer/` folder
  - Implement `IRequestHandler<UpdateCustomerCommand, BaseResult<bool>>`
  - Inject: `UpdateCustomerCommandValidator`, `CustomerDbContext`, `ICacheService`
  - Flow in `HandleAsync`:
    1. `await _validator.ValidateAsync(request, cancellationToken)` -> return `Failure(validationResult.ToDictionary())` if invalid
    2. `await _customerDbContext.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)` -> return `Failure("Customer does not exist!")` if null
    3. `customer.Update(request.LegalName, request.TradeName, request.Email, request.MainPhone, request.SecondaryPhone, request.SiteUrl, request.MainContactName)`
    4. `await _customerDbContext.SaveChangesAsync(cancellationToken)`
    5. `await _cacheService.RemoveAsync(CacheKeys.Customer(customer.Id), cancellationToken)`
    6. `return BaseResult<bool>.Success(true)`
  - _Requirements: 1.1, 1.4, 1.5, 2.12, 3.1, 3.2_

- [x] 5. Checkpoint -- verify compilation
  - Ensure the project compiles without errors

- [x] 6. Create `UpdateCustomerEndpoint`
  - Create `UpdateCustomerEndpoint.cs` in the `Features/UpdateCustomer/` folder
  - Inherit `BaseController`
  - Decorate with `[Route("api/customer")]` and `[Tags("Customers")]`
  - Method `UpdateCustomerAsync([FromRoute] Guid id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)`:
    - Assign `command.Id = id` before sending to the mediator
    - `BaseResult<bool> result = await _mediator.SendAsync(command, cancellationToken)`
    - Return `result.IsValid ? Ok() : BadRequestProblem(result.Errors)`
  - Add attributes:
    - `[HttpPut("{id:guid}")]`
    - `[ProducesResponseType(StatusCodes.Status200OK)]`
    - `[ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]`
  - XML documentation: `<summary>`, `<param name="id">`, `<param name="command">`, `<returns>`, `<remarks>` with a sample request body
  - _Requirements: 4.1-4.6_

- [x] 7. Final checkpoint
  - Ensure the project compiles without errors after wiring the endpoint

## Notes

- No EF Core migration is needed -- all fields already exist in the table
- The validator does **not** check customer existence in the database (Requirement 2.13)
- The `Id` in the command uses `set` + `[JsonIgnore]` because it comes from the route, not the body