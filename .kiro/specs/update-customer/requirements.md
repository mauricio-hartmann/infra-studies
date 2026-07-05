# Requirements Document

## Introduction

This feature adds to `IS.Customers.API` an endpoint to update the registration data of an existing customer. The updatable fields are: LegalName, TradeName, Email, MainPhone, SecondaryPhone, SiteUrl, and MainContactName. Derived computed fields (NormalizedLegalName, NormalizedTradeName) are updated automatically by the entity. RegistrationNumber is not editable.

## Glossary

- **API**: The `IS.Customers.API` service.
- **Customer**: Domain entity representing a registered customer in the system.
- **Command**: Immutable object that encapsulates the intent to mutate state, implementing `ICommand<TResponse>`.
- **Handler**: Class responsible for executing the use case, receiving a Command and returning `BaseResult<T>`.
- **Validator**: FluentValidation class that validates the Command before any database operation.
- **BaseResult**: Result wrapper with `IsValid`, `Response`, and `Errors`.
- **CacheService**: Redis cache service (`ICacheService`) for get/set/remove operations.
- **NormalizedName**: Version of a text field without diacritics, without non-alphanumeric characters, with collapsed whitespace and uppercased, generated via `NormalizeToUpper()`.

---

## Requirements

### Requirement 1: Customer data update

**User Story:** As a system operator, I want to update the registration data of an existing customer, so that the customer's information remains correct and up to date.

#### Acceptance Criteria

1. WHEN an update request is received with a valid customer `Id` and valid fields, THE API SHALL persist the changes and return HTTP 200 OK.
2. WHEN the LegalName or TradeName fields are provided in the request, THE API SHALL also update the NormalizedLegalName and NormalizedTradeName fields respectively, applying `NormalizeToUpper()`.
3. WHEN an update request is received, THE API SHALL accept the customer `Id` exclusively via route parameter (URL).
4. WHEN the Handler processes the command, THE Handler SHALL fetch the customer from the database before applying any changes.
5. IF the customer with the given `Id` is not found in the database, THEN THE Handler SHALL return `BaseResult<bool>.Failure` with the message "Customer does not exist!" without persisting any changes.

---

### Requirement 2: Input validation

**User Story:** As a system operator, I want required fields and size limits to be validated before persistence, so that invalid data is never written to the database.

#### Acceptance Criteria

1. THE Validator SHALL reject commands where LegalName is empty or null, returning an error with the name "Legal name".
2. THE Validator SHALL reject commands where LegalName exceeds 255 characters.
3. THE Validator SHALL reject commands where TradeName is empty or null, returning an error with the name "Trade name".
4. THE Validator SHALL reject commands where TradeName exceeds 255 characters.
5. THE Validator SHALL reject commands where MainPhone is empty or null, returning an error with the name "Main phone".
6. THE Validator SHALL reject commands where MainPhone exceeds 50 characters.
7. WHEN Email is provided, THE Validator SHALL reject commands where Email exceeds 100 characters.
8. WHEN SecondaryPhone is provided, THE Validator SHALL reject commands where SecondaryPhone exceeds 50 characters, returning an error with the name "Secondary phone".
9. WHEN SiteUrl is provided, THE Validator SHALL reject commands where SiteUrl exceeds 100 characters, returning an error with the name "Site URL".
10. THE Validator SHALL reject commands where MainContactName is empty or null, returning an error with the name "Main contact name".
11. THE Validator SHALL reject commands where MainContactName exceeds 255 characters.
12. IF any validation rule fails, THEN THE Handler SHALL return `BaseResult<bool>.Failure` with the FluentValidation error dictionary and not persist any changes.
13. THE Validator SHALL NOT check for customer existence in the database -- that responsibility belongs exclusively to the Handler.

---

### Requirement 3: Cache invalidation

**User Story:** As a system operator, I want the customer cache to be invalidated after a successful update, so that subsequent reads reflect the most recent data.

#### Acceptance Criteria

1. WHEN the customer update is successfully persisted, THE Handler SHALL invoke `ICacheService.RemoveAsync(CacheKeys.Customer(id), cancellationToken)` to invalidate the cache entry for the updated customer.
2. THE Handler SHALL invoke cache invalidation only after `SaveChangesAsync` completes without errors.

---

### Requirement 4: Endpoint contract

**User Story:** As an API consumer, I want a well-defined and documented endpoint to update customers, so that I can integrate it correctly.

#### Acceptance Criteria

1. THE API SHALL expose the update endpoint at route `PUT api/customer/{id:guid}`.
2. THE Endpoint SHALL declare `[ProducesResponseType(StatusCodes.Status200OK)]` for the success scenario.
3. THE Endpoint SHALL declare `[ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]` for validation or business failures.
4. THE Endpoint SHALL receive the request body via `[FromBody]` and the `Id` via `[FromRoute]`.
5. THE Endpoint SHALL propagate the `CancellationToken` to all async calls.
6. THE Endpoint SHALL include XML documentation with `<summary>`, `<param>`, `<returns>`, and `<remarks>` containing a sample request body.