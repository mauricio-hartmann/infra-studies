using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace IS.Core.Cache.Configuration
{
    public static class CacheConfiguration
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration, string connectionString, string instanceName)
        {
            string redisConnection = configuration.GetConnectionString(connectionString) 
                                     ?? throw new InvalidOperationException($"Connection string '{connectionString}' was not found.");

            ConfigurationOptions redisOptions = ConfigurationOptions.Parse(redisConnection);
            redisOptions.AbortOnConnectFail = false;
            redisOptions.ConnectTimeout = 2_000;
            redisOptions.AsyncTimeout = 1_000;
            redisOptions.SyncTimeout = 1_000;
            redisOptions.BacklogPolicy = BacklogPolicy.FailFast;
            redisOptions.ConnectRetry = 1;
            redisOptions.KeepAlive = 30;
            redisOptions.ReconnectRetryPolicy = new ExponentialRetry(3_000);

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = redisOptions;
                options.InstanceName = instanceName;
            });

            services.AddScoped<ICacheService, CacheService>();
            
            return services;
        }
    }
}
