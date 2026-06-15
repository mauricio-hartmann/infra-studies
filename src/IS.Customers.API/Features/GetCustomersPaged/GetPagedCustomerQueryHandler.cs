using FluentValidation.Results;
using IS.Core.Communication;
using IS.Core.Data.Extensions;
using IS.Core.Data.Pagination;
using IS.Core.Extensions;
using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data;
using IS.Customers.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Features.GetCustomersPaged
{
    public class GetPagedCustomerQueryHandler : IRequestHandler<GetPagedCustomersQuery, BaseResult<PagedResult<PagedCustomerDTO>>>
    {
        private readonly GetPagedCustomersQueryValidator _validator;
        private readonly CustomerDbContext _customerDbContext;

        public GetPagedCustomerQueryHandler(GetPagedCustomersQueryValidator validator, CustomerDbContext customerDbContext)
        {
            _validator = validator;
            _customerDbContext = customerDbContext;
        }

        public async Task<BaseResult<PagedResult<PagedCustomerDTO>>> HandleAsync(GetPagedCustomersQuery request, CancellationToken cancellationToken = default)
        {
            ValidationResult validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
                return BaseResult<PagedResult<PagedCustomerDTO>>.Failure(validationResult.ToDictionary());

            PagedResult<PagedCustomerDTO> pagedResult = await QueryAsync(request.PageNumber, request.PageSize, request.Filter, cancellationToken);

            return BaseResult<PagedResult<PagedCustomerDTO>>.Success(pagedResult);
        }

        private async Task<PagedResult<PagedCustomerDTO>> QueryAsync(int pageNumber, int pageSize, string filter, CancellationToken cancellationToken)
        {
            IQueryable<Customer> queryable = _customerDbContext.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.NormalizeToUpper();
                queryable = queryable.Where(x => EF.Functions.ILike(x.NormalizedLegalName, $"%{filter}%") || EF.Functions.ILike(x.NormalizedTradeName, $"%{filter}%"));
            }

            return await queryable.OrderBy(x => x.TradeName)
                .ThenBy(x => x.Id)
                .Select(x => new PagedCustomerDTO
                {
                    Id = x.Id,
                    LegalName = x.LegalName,
                    TradeName = x.TradeName,
                    RegistrationNumber = x.RegistrationNumber
                })
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
