using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IS.Core.Cache.Configuration
{
    public static class CacheConfiguration
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration, string connectionString, string instanceName)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString(connectionString);
                options.InstanceName = instanceName;
            });
            services.AddScoped<ICacheService, CacheService>();
            
            return services;
        }
    }
}
