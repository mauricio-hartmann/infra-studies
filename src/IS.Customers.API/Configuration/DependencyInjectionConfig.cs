using FluentValidation;
using IS.Core.Data;
using IS.Customers.API.Data;
using IS.Customers.API.Features.CreateCustomer;

namespace IS.Customers.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependenciesConfiguration(this IServiceCollection services)
        {
            // background services
            services.AddHostedService<RelationalDatabaseMigrationService<CustomerDbContext>>();

            // validators
            services.AddValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>();

            return services;
        }
    }
}
