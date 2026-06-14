using FluentValidation;
using IS.Customers.API.Background;
using IS.Customers.API.Features.CreateCustomer;

namespace IS.Customers.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependenciesConfiguration(this IServiceCollection services)
        {
            // background services
            services.AddHostedService<PostgresMigrationService>();

            // validators
            services.AddValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>();

            return services;
        }
    }
}
