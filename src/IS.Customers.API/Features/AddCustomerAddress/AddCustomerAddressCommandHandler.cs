using FluentValidation.Results;
using IS.Core.Cache;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data;
using IS.Customers.API.Entities;
using IS.Customers.API.Shared;
using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Features.AddCustomerAddress
{
    public class AddCustomerAddressCommandHandler : IRequestHandler<AddCustomerAddressCommand, BaseResult<Guid>>
    {
        private readonly AddCustomerAddressCommandValidator _validator;
        private readonly CustomerDbContext _customerDbContext;
        private readonly ICacheService _cacheService;

        public AddCustomerAddressCommandHandler(AddCustomerAddressCommandValidator validator,
                                                CustomerDbContext customerDbContext,
                                                ICacheService cacheService)
        {
            _validator = validator;
            _customerDbContext = customerDbContext;
            _cacheService = cacheService;
        }

        public async Task<BaseResult<Guid>> HandleAsync(AddCustomerAddressCommand request, CancellationToken cancellationToken = default)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BaseResult<Guid>.Failure(validationResult.ToDictionary());

            Customer customer = await GetCustomerByIdAsync(request.CustomerId, cancellationToken);

            if (customer is null)
                return BaseResult<Guid>.Failure("Customer does not exists!");

            Address address = CreateAddress(request);
            customer.AddAddress(address, request.IsMainAddress);

            await _customerDbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.Customer(customer.Id), cancellationToken);

            return BaseResult<Guid>.Success(address.Id);
        }

        private async Task<Customer> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _customerDbContext.Customers
                .Include(x => x.Addresses)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        private static Address CreateAddress(AddCustomerAddressCommand command)
        {
            return new Address(command.Street,
                               command.Number,
                               command.AddressComplement ?? string.Empty,
                               command.City,
                               command.State,
                               command.Country,
                               command.IsMainAddress);
        }
    }
}
