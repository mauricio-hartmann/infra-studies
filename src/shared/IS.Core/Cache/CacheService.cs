using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace IS.Core.Cache
{
    internal class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken)
        {
            try
            {
                byte[] cachedResponse = await _cache.GetAsync(cacheKey, cancellationToken);

                return cachedResponse == null ? default
                                              : JsonSerializer.Deserialize<T>(cachedResponse);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Cache GET failed for key {CacheKey}. Falling through to source.", cacheKey);
                return default;
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Cache REMOVE failed for key {CacheKey}.", key);
            }
        }

        public async Task SetAsync<T>(string cacheKey, T data, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            try
            {
                byte[] serializedData = Encoding.Default.GetBytes(JsonSerializer.Serialize(data));
                await _cache.SetAsync(cacheKey, serializedData, options, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Cache SET failed for key {CacheKey}.", cacheKey);
            }
        }
    }
}
