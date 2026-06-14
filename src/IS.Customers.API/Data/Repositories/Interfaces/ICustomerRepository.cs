using IS.Customers.API.Entities;
using IS.Customers.API.Features.GetCustomerById;

namespace IS.Customers.API.Data.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> ExistsByRegistrationNumberAsync(string RegistrationNumber, CancellationToken cancellationToken);
        Task AddAsync(Customer customer, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<CustomerDTO> ByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
