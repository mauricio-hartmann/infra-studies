using IS.Core.Communication;
using IS.Core.Data.Pagination;
using IS.Core.Mediator.Interfaces;

namespace IS.Customers.API.Features.GetCustomersPaged
{
    public record GetPagedCustomersQuery : PaginationParameters, 
                                           IQuery<BaseResult<PagedResult<PagedCustomerDTO>>>
    {
        public string? Filter { get; init; }
    }
}
