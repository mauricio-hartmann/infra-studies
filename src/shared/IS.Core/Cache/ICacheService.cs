using Microsoft.Extensions.Caching.Distributed;

namespace IS.Core.Cache
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken);
        Task SetAsync<T>(string cacheKey, T data, DistributedCacheEntryOptions options, CancellationToken cancellationToken);
        Task RemoveAsync(string key, CancellationToken cancellationToken);
    }
}
