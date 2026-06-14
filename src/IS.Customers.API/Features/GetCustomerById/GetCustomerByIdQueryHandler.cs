using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data.Repositories.Interfaces;

namespace IS.Customers.API.Features.GetCustomerById
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDTO>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDTO> HandleAsync(GetCustomerByIdQuery request, CancellationToken cancellationToken = default)
        {
            return await _customerRepository.ByIdAsync(request.Id, cancellationToken);
        }
    }
}
