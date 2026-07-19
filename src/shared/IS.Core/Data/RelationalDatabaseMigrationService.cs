using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IS.Core.Data;

public class RelationalDatabaseMigrationService<T> : BackgroundService where T : DbContext
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RelationalDatabaseMigrationService<T>> _logger;

    public RelationalDatabaseMigrationService(IServiceScopeFactory serviceScopeFactory, ILogger<RelationalDatabaseMigrationService<T>> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        DbContext customerDbContext = serviceScope.ServiceProvider.GetRequiredService<T>();

        _logger.LogInformation("Executando migrations");
        await customerDbContext.Database.MigrateAsync(stoppingToken);
    }
}
