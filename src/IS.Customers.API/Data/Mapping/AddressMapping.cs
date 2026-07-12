using IS.Customers.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IS.Customers.API.Data.Mapping
{
    public class AddressMapping : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Street)
                .IsRequired();

            builder.Property(x => x.Number)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.AddressComplement)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.State)
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(x => x.Country)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId);

            builder.ToTable("Addresses");
        }
    }
}
