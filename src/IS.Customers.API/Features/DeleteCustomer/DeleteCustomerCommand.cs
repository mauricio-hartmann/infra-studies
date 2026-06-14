using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;

namespace IS.Customers.API.Features.DeleteCustomer
{
    public record DeleteCustomerCommand : ICommand<BaseResult<bool>>
    {
        public Guid Id { get; init; }

        public DeleteCustomerCommand(Guid id)
        {
            Id = id;
        }
    }
}
