using IS.Core.Data.Extensions;
using IS.Core.Messaging.Outbox;
using IS.Core.Messaging.Outbox.Data;
using IS.Customers.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IS.Customers.API.Data;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; init; }
    public DbSet<OutboxMessage> OutboxMessages { get; init; }
    public DbSet<OutboxPublishAttempt> OutboxPublishAttempts { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.SetDefaultConfiguration(typeof(CustomerDbContext).Assembly)
                    .AddOutbox();

        #region DateDeleted filter
        modelBuilder.Entity<Customer>().HasQueryFilter(x => !x.DateDeleted.HasValue);
        modelBuilder.Entity<Address>().HasQueryFilter(x => !x.DateDeleted.HasValue);
        #endregion

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.SetDefaultConfigurationConventions();
    }
}
