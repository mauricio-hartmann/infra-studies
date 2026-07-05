using FluentValidation.Results;
using IS.Core.Cache;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data;
using IS.Customers.API.Entities;
using IS.Customers.API.Shared;
using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Features.DeleteCustomerAddress
{
    public class DeleteCustomerAddressCommandHandler : IRequestHandler<DeleteCustomerAddressCommand, BaseResult<bool>>
    {
        private readonly DeleteCustomerAddressCommandValidator _validator;
        private readonly CustomerDbContext _customerDbContext;
        private readonly ICacheService _cacheService;

        public DeleteCustomerAddressCommandHandler(DeleteCustomerAddressCommandValidator validator,
                                                   CustomerDbContext customerDbContext,
                                                   ICacheService cacheService)
        {
            _validator = validator;
            _customerDbContext = customerDbContext;
            _cacheService = cacheService;
        }

        public async Task<BaseResult<bool>> HandleAsync(DeleteCustomerAddressCommand request, CancellationToken cancellationToken = default)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BaseResult<bool>.Failure(validationResult.ToDictionary());

            Customer? customer = await GetCustomerWithAddressesAsync(request.CustomerId, cancellationToken);

            if (customer is null)
                return BaseResult<bool>.Failure("Customer does not exist!");

            if (request.NewMainAddressId.HasValue)
            {
                bool newMainIsValid = customer.Addresses.Any(a => a.Id == request.NewMainAddressId.Value);

                if (!newMainIsValid)
                    return BaseResult<bool>.Failure("New main address does not exist or does not belong to this customer!");
            }

            bool deleted = customer.DeleteAddress(request.AddressId, request.NewMainAddressId);

            if (!deleted)
                return BaseResult<bool>.Failure("Address does not exist or does not belong to this customer!");

            await _customerDbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(ChacheKeys.Customer(customer.Id), cancellationToken);

            return BaseResult<bool>.Success(true);
        }

        private async Task<Customer?> GetCustomerWithAddressesAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return await _customerDbContext.Customers
                .Include(x => x.Addresses)
                .FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken);
        }
    }
}
