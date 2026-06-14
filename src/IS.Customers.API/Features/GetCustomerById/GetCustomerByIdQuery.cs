using IS.Core.Mediator.Interfaces;

namespace IS.Customers.API.Features.GetCustomerById
{
    public class GetCustomerByIdQuery : IQuery<CustomerDTO>
    {
        public Guid Id { get; init; }

        public GetCustomerByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
