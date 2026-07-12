using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;

namespace IS.Customers.API.Features.AddCustomerAddress
{
    public class AddCustomerAddressCommand : ICommand<BaseResult<Guid>>
    {
        public Guid CustomerId { get; set; }
        public string Street { get; init; } = string.Empty;
        public string Number { get; init; } = string.Empty;
        public string AddressComplement { get; init; }
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public bool IsMainAddress { get; init; }
    }
}
