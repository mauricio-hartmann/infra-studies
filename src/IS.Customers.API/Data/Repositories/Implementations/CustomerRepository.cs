using IS.Customers.API.Data.Repositories.Interfaces;
using IS.Customers.API.Entities;
using IS.Customers.API.Features.GetCustomerById;
using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Data.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _customerDbContext;

        public CustomerRepository(CustomerDbContext customerDbContext)
        {
            _customerDbContext = customerDbContext;
        }

        public async Task<bool> ExistsByRegistrationNumberAsync(string RegistrationNumber, CancellationToken cancellationToken)
        {
            return await _customerDbContext.Customers
                .AnyAsync(x => x.RegistrationNumber == RegistrationNumber, cancellationToken);
        }

        public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
        {
            await _customerDbContext.Customers.AddAsync(customer, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _customerDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<CustomerDTO> ByIdAsync(Guid id, CancellationToken cancellationToken)
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