using IS.Customers.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IS.Customers.API.Data.Mapping
{
    public class CustomerMapping : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.LegalName)
                .IsRequired();

            builder.Property(x => x.NormalizedLegalName)
                .IsRequired();

            builder.Property(x => x.TradeName)
                .IsRequired();

            builder.Property(x => x.NormalizedTradeName)
                .IsRequired();

            builder.Property(x => x.RegistrationNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(100);

            builder.Property(x => x.MainPhone)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.SecondaryPhone)
                .HasMaxLength(50);

            builder.Property(x => x.SiteUrl)
                .HasMaxLength(100);

            builder.HasIndex(x => x.RegistrationNumber);

            builder.HasIndex(x => x.NormalizedLegalName);

            builder.HasIndex(x => x.NormalizedLegalName)
                .HasDatabaseName("IX_Customers_NormalizedLegalName_Trgm")
                .HasMethod("gin")
                .HasOperators("gin_trgm_ops");

            builder.HasIndex(x => x.NormalizedTradeName);

            builder.HasIndex(x => x.NormalizedTradeName)
                .HasDatabaseName("IX_Customers_NormalizedTradeName_Trgm")
                .HasMethod("gin")
                .HasOperators("gin_trgm_ops");

            builder.HasMany(x => x.Addresses)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId);

            builder.ToTable("Customers");
        }
    }
}
