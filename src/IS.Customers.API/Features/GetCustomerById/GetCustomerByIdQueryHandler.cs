using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data;
using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Features.GetCustomerById
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDTO?>
    {
        private readonly CustomerDbContext _customerDbContext;

        public GetCustomerByIdQueryHandler(CustomerDbContext customerDbContext)
        {
            _customerDbContext = customerDbContext;
        }

        public async Task<CustomerDTO?> HandleAsync(GetCustomerByIdQuery request, CancellationToken cancellationToken = default)
        {
            return await ByIdAsync(request.Id, cancellationToken);
        }

        private async Task<CustomerDTO?> ByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _customerDbContext.Customers
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CustomerDTO
                {
                    Id = x.Id,
                    LegalName = x.LegalName,
                    TradeName = x.TradeName,
                    RegistrationNumber = x.RegistrationNumber,
                    Email = x.Email,
                    MainPhone = x.MainPhone,
                    SecondaryPhone = x.SecondaryPhone,
                    SiteUrl = x.SiteUrl,
                    MainContactName = x.MainContactName,
                    Addresses = x.Addresses.Select(a => new AddressDTO
                    {
                        Id = a.Id,
                        Street = a.Street,
                        Number = a.Number,
                        AddressComplement = a.AddressComplement,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        IsMainAddress = a.IsMainAddress,
                    })
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
