using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data;
using IS.Customers.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Features.DeleteCustomer
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, BaseResult<bool>>
    {
        private readonly CustomerDbContext _customerDbContext;

        public DeleteCustomerCommandHandler(CustomerDbContext customerDbContext)
        {
            _customerDbContext = customerDbContext;
        }

        public async Task<BaseResult<bool>> HandleAsync(DeleteCustomerCommand request, CancellationToken cancellationToken = default)
        {
            Customer? customer = await GetCustomerByIdAsync(request.Id, cancellationToken);

            if (customer is null)
                return BaseResult<bool>.Failure("Customer does not exists!");

            customer.Delete();
            await _customerDbContext.SaveChangesAsync(cancellationToken);

            return BaseResult<bool>.Success(true);
        }

        private async Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _customerDbContext.Customers
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
