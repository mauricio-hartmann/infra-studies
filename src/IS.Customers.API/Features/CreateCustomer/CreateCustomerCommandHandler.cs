using FluentValidation.Results;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data;
using IS.Customers.API.Entities;

namespace IS.Customers.API.Features.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, BaseResult<Guid>>
    {
        private readonly ILogger<CreateCustomerCommandHandler> _logger;
        private readonly CreateCustomerCommandValidator _validator;
        private readonly CustomerDbContext _customerDbContext;

        public CreateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger,
                                            CreateCustomerCommandValidator validator,
                                            CustomerDbContext customerDbContext)
        {
            _logger = logger;
            _validator = validator;
            _customerDbContext = customerDbContext;
        }

        public async Task<BaseResult<Guid>> HandleAsync(CreateCustomerCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BaseResult<Guid>.Failure(validationResult.ToDictionary());

                Customer customer = CreateCustomer(request);
                await SaveAsync(customer, cancellationToken);

                return BaseResult<Guid>.Success(customer.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BaseResult<Guid>.Failure("Unexpected error creating customer!");
            }
        }

        private Customer CreateCustomer(CreateCustomerCommand command)
        {
            return new Customer(command.LegalName, command.TradeName, command.RegistrationNumber)
            {
                Email = command.Email,
                MainPhone = command.MainPhone,
                SecondaryPhone = command.SecondaryPhone,
                SiteUrl = command.SiteUrl,
                MainContactName = command.MainContactName
            };
        }

        private async Task SaveAsync(Customer customer, CancellationToken cancellationToken)
        {
            await _customerDbContext.AddAsync(customer, cancellationToken);
            await _customerDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
