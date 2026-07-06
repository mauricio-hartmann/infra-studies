using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using System.Text.Json.Serialization;

namespace IS.Customers.API.Features.UpdateCustomerAddress
{
    public record UpdateCustomerAddressCommand : ICommand<BaseResult<bool>>
    {
        [JsonIgnore]
        public Guid CustomerId { get; set; }

        [JsonIgnore]
        public Guid AddressId { get; set; }

        public string Street { get; init; } = string.Empty;
        public string Number { get; init; } = string.Empty;
        public string? AddressComplement { get; init; }
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public bool IsMainAddress { get; init; }
    }
}
