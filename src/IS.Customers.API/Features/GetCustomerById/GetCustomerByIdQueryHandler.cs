using IS.Core.Cache;
using IS.Core.Mediator.Interfaces;
using IS.Customers.API.Data;
using IS.Customers.API.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace IS.Customers.API.Features.GetCustomerById
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDTO>
    {
        private readonly CustomerDbContext _customerDbContext;
        private readonly ICacheService _cacheService;

        public GetCustomerByIdQueryHandler(CustomerDbContext customerDbContext, ICacheService cacheService)
        {
            _customerDbContext = customerDbContext;
            _cacheService = cacheService;
        }

        public async Task<CustomerDTO> HandleAsync(GetCustomerByIdQuery request, CancellationToken cancellationToken = default)
        {
            CustomerDTO customerFromCache = await GetFromCacheAsync(request.Id, cancellationToken);

            if (customerFromCache != null) return customerFromCache;

            CustomerDTO customer = await ByIdAsync(request.Id, cancellationToken);

            if (customer != null) await SetCacheAsync(customer, cancellationToken);

            return customer;
        }

        private async Task<CustomerDTO> GetFromCacheAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _cacheService.GetAsync<CustomerDTO>(CacheKeys.Customer(id), cancellationToken);
        }

        private async Task SetCacheAsync(CustomerDTO customer, CancellationToken cancellationToken)
        {
            var distributedCacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
            await _cacheService.SetAsync(CacheKeys.Customer(customer.Id), customer, distributedCacheEntryOptions, cancellationToken);
        }

        private async Task<CustomerDTO> ByIdAsync(Guid id, CancellationToken cancellationToken)
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
