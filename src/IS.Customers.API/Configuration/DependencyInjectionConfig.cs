using IS.Customers.API.Background;

namespace IS.Customers.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependenciesConfiguration(this IServiceCollection services)
        {
            // background services
            services.AddHostedService<PostgresMigrationService>();

            return services;
        }
    }
}
