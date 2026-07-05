using FluentValidation.Results;
using IS.Core.Cache;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data;
using IS.Customers.API.Entities;
using IS.Customers.API.Shared;
using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Features.UpdateCustomer
{
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

            Customer customer = await _customerDbContext.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (customer == null)
                return BaseResult<bool>.Failure("Customer does not exist!");

            customer.Update(request.LegalName, request.TradeName, request.Email, request.MainPhone, request.SecondaryPhone, request.SiteUrl, request.MainContactName);

            await _customerDbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.Customer(customer.Id), cancellationToken);

            return BaseResult<bool>.Success(true);
        }
    }
}
