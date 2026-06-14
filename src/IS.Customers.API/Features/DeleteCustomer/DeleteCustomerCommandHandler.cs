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
        private readonly ILogger<DeleteCustomerCommandHandler> _logger;

        public DeleteCustomerCommandHandler(CustomerDbContext customerDbContext, ILogger<DeleteCustomerCommandHandler> logger)
        {
            _customerDbContext = customerDbContext;
            _logger = logger;
        }

        public async Task<BaseResult<bool>> HandleAsync(DeleteCustomerCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                Customer? customer = await GetCustomerByIdAsync(request.Id, cancellationToken);

                if (customer is null)
                    return BaseResult<bool>.Failure("Customer does not exists!");

                customer.Delete();
                await _customerDbContext.SaveChangesAsync(cancellationToken);

                return BaseResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BaseResult<bool>.Failure("Unexpected error deleting customer.");
            }
        }

        private async Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _customerDbContext.Customers
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
