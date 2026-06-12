using IS.Customers.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace IS.Customers.API.Background
{
    public class PostgresMigrationService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PostgresMigrationService> _logger;

        public PostgresMigrationService(IServiceScopeFactory serviceScopeFactory, ILogger<PostgresMigrationService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
            CustomerDbContext customerDbContext = serviceScope.ServiceProvider.GetRequiredService<CustomerDbContext>();

            _logger.LogInformation("Executando migrations");
            await customerDbContext.Database.MigrateAsync(stoppingToken);
        }
    }
}
