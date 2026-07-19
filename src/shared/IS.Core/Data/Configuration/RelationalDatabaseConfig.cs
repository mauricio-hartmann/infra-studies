using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IS.Core.Data.Configuration;

public static class RelationalDatabaseConfig
{
    public static IServiceCollection AddDbContext<T>(this IServiceCollection services, string connectionStringId, IHostEnvironment env) where T : DbContext
    {
        services.AddDbContext<T>((provider, options) =>
        {
            string connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString(connectionStringId)!;
            options.UseNpgsql(connectionString, p => p.EnableRetryOnFailure(maxRetryCount: 2, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null));

            if (env.IsDevelopment())
                options.EnableSensitiveDataLogging();
        });

        return services;
    }
}
