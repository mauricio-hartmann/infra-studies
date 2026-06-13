using IS.Customers.API.Background;
using IS.Customers.API.Data.Repositories.Implementations;
using IS.Customers.API.Data.Repositories.Interfaces;

namespace IS.Customers.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependenciesConfiguration(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            // background services
            services.AddHostedService<PostgresMigrationService>();

            return services;
        }
    }
}
