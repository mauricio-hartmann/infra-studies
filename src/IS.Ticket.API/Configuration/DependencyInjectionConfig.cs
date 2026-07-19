using IS.Core.Data;
using IS.Ticket.API.Data;

namespace IS.Ticket.API.Configuration;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddDependenciesConfiguration(this IServiceCollection services)
    {
        // background services
        services.AddHostedService<RelationalDatabaseMigrationService<TicketDbContext>>();

        return services;
    }
}