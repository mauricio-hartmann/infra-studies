using IS.Ticket.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IS.Ticket.API.Data.Mapping;

public class CustomerMapping : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LegalName)
            .IsRequired();

        builder.Property(x => x.TradeName)
            .IsRequired();

        builder.Property(x => x.RegistrationNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.ToTable("Customers");
    }
}
