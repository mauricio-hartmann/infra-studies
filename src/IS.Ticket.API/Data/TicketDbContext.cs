using IS.Core.Data.Extensions;
using IS.Ticket.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IS.Ticket.API.Data
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SetDefaultConfiguration(typeof(TicketDbContext).Assembly);

            #region DateDeleted filter
            modelBuilder.Entity<Customer>().HasQueryFilter(x => !x.DateDeleted.HasValue);
            #endregion

            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.SetDefaultConfigurationConventions();
        }
    }
}
