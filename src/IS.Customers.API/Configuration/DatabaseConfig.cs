using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Configuration
{
    public static class DatabaseConfig
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
}
