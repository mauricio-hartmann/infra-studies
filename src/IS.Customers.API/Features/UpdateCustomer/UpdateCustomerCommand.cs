using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using System.Text.Json.Serialization;

namespace IS.Customers.API.Features.UpdateCustomer
{
    public record UpdateCustomerCommand : ICommand<BaseResult<bool>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string LegalName { get; init; }
        public string TradeName { get; init; }
        public string Email { get; init; }
        public string MainPhone { get; init; }
        public string SecondaryPhone { get; init; }
        public string SiteUrl { get; init; }
        public string MainContactName { get; init; }
    }
}
