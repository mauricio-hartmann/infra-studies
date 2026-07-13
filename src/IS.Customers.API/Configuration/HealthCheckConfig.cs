using IS.Core.HealthCheck;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IS.Customers.API.Configuration;

public static class HealthCheckConfig
{
    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
                .AddCheck("Application", () => HealthCheckResult.Healthy(), tags: ["live"])
                .AddCheck(
                    "Database-Check",
                    new PostgreSqlConnectionHealthCheck(configuration["ConnectionStrings:PostgresConnection"]),
                    HealthStatus.Unhealthy,
                    ["CustomersDB", "database", "ready"]
                );

        return services;
    }

    public static WebApplication MapHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("live")});
        app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("ready") });

        return app;
    }
}
