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

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository,
                                            ILogger<CreateCustomerCommandHandler> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<BaseResult<Guid>> HandleAsync(CreateCustomerCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                bool customerAlreadyExists = await _customerRepository
                    .ExistsByRegistrationNumberAsync(request.RegistrationNumber, cancellationToken);

                if (customerAlreadyExists)
                    return BaseResult<Guid>.Failure("A customer with same registration number already exists!");

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
