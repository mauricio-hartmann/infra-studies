using IS.Customers.API.Data.Repositories.Interfaces;
using IS.Customers.API.Entities;
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
    }
}