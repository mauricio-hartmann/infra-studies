using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IS.Core.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder SetDefaultConfiguration(this ModelBuilder modelBuilder, Assembly configurationsAssembly)
        {
            modelBuilder.HasPostgresExtension("pg_trgm");
            modelBuilder.ApplyConfigurationsFromAssembly(configurationsAssembly);

            return modelBuilder;
        }

        public static void SetDefaultConfigurationConventions(this ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp with time zone");
            configurationBuilder.Properties<DateTime?>().HaveColumnType("timestamp with time zone");
            configurationBuilder.Properties<string>().HaveMaxLength(255);
        }
    }
}
