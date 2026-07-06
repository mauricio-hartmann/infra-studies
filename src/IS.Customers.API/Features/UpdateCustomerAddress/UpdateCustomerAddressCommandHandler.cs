using FluentValidation.Results;
using IS.Core.Cache;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data;
using IS.Customers.API.Entities;
using IS.Customers.API.Shared;
using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Features.UpdateCustomerAddress
{
    public class UpdateCustomerAddressCommandHandler : IRequestHandler<UpdateCustomerAddressCommand, BaseResult<bool>>
    {
        private readonly UpdateCustomerAddressCommandValidator _validator;
        private readonly CustomerDbContext _customerDbContext;
        private readonly ICacheService _cacheService;

        public UpdateCustomerAddressCommandHandler(UpdateCustomerAddressCommandValidator validator,
                                                   CustomerDbContext customerDbContext,
                                                   ICacheService cacheService)
        {
            _validator = validator;
            _customerDbContext = customerDbContext;
            _cacheService = cacheService;
        }

        public async Task<BaseResult<bool>> HandleAsync(UpdateCustomerAddressCommand request, CancellationToken cancellationToken = default)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BaseResult<bool>.Failure(validationResult.ToDictionary());

            Customer? customer = await GetCustomerWithAddressesAsync(request.CustomerId, cancellationToken);

            if (customer is null)
                return BaseResult<bool>.Failure("Customer does not exist!");

            BaseResult<bool> updateResult = customer.UpdateAddress(request.AddressId,
                                                                   request.Street,
                                                                   request.Number,
                                                                   request.AddressComplement,
                                                                   request.City,
                                                                   request.State,
                                                                   request.Country,
                                                                   request.IsMainAddress);

            if (!updateResult.IsValid)
                return updateResult;

            await _customerDbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.Customer(customer.Id), cancellationToken);

            return updateResult;
        }

        private async Task<Customer?> GetCustomerWithAddressesAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return await _customerDbContext.Customers
                .Include(x => x.Addresses)
                .FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken);
        }
    }
}
