using FluentValidation.Results;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data.Repositories.Interfaces;
using IS.Customers.API.Entities;

namespace IS.Customers.API.Features.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, BaseResult<Guid>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CreateCustomerCommandHandler> _logger;
        private readonly CreateCustomerCommandValidator _validator;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository,
                                            ILogger<CreateCustomerCommandHandler> logger,
                                            CreateCustomerCommandValidator validator)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<BaseResult<Guid>> HandleAsync(CreateCustomerCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BaseResult<Guid>.Failure(validationResult.ToDictionary());

                var customer = new Customer(request.LegalName, request.TradeName, request.RegistrationNumber)
                {
                    Email = request.Email,
                    MainPhone = request.MainPhone,
                    SecondaryPhone = request.SecondaryPhone,
                    SiteUrl = request.SiteUrl,
                    MainContactName = request.MainContactName
                };

                await _customerRepository.AddAsync(customer, cancellationToken);
                await _customerRepository.SaveChangesAsync(cancellationToken);

                return BaseResult<Guid>.Success(customer.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BaseResult<Guid>.Failure("Unexpected error creating customer!");
            }
        }
    }
}
