using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;

namespace IS.Customers.API.Features.CreateCustomer
{
    public class CreateCustomerCommand : ICommand<BaseResult<Guid>>
    {
        public string LegalName { get; init; }
        public string TradeName { get; init; }
        public string RegistrationNumber { get; init; }
        public string Email { get; init; }
        public string MainPhone { get; init; }
        public string SecondaryPhone { get; init; }
        public string SiteUrl { get; init; }
        public string MainContactName { get; init; }
    }
}
