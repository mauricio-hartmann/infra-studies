using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IS.Core.Messaging.Outbox.Data;

public sealed class OutboxMessageMapping : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType)
            .IsRequired();

        builder.Property(x => x.Payload)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.AttemptCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.AggregateId)
            .IsRequired();

        builder.HasMany(x => x.OutboxPublishAttempts)
            .WithOne(opa => opa.OutboxMessage)
            .HasForeignKey(opa => opa.OutboxMessageId);

        builder.HasIndex(x => new { x.Status, x.NextAttemptAtUtc });

        builder.HasIndex(x => new { x.Status, x.PublishingExpiresAtUtc });

        builder.ToTable("OutboxMessages");
    }
}
