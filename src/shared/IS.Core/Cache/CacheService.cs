using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace IS.Core.Cache
{
    internal class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken)
        {
            byte[] cachedResponse = await _cache.GetAsync(cacheKey, cancellationToken);

            return cachedResponse == null ? default
                                          : JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task SetAsync<T>(string cacheKey, T data, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            byte[] serializedData = Encoding.Default.GetBytes(JsonSerializer.Serialize(data));
            await _cache.SetAsync(cacheKey, serializedData, options, cancellationToken);
        }
    }
}
