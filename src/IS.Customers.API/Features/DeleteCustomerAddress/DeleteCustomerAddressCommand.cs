using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;

namespace IS.Customers.API.Features.DeleteCustomerAddress
{
    public class DeleteCustomerAddressCommand : ICommand<BaseResult<bool>>
    {
        public Guid CustomerId { get; set; }
        public Guid AddressId { get; set; }
        public Guid? NewMainAddressId { get; init; }
    }
}
