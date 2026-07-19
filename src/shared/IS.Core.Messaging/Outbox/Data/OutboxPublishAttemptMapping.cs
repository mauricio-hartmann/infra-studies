using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IS.Core.Messaging.Outbox.Data;

public sealed class OutboxPublishAttemptMapping : IEntityTypeConfiguration<OutboxPublishAttempt>
{
    public void Configure(EntityTypeBuilder<OutboxPublishAttempt> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AttemptNumber)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PublishingBy)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(500);

        builder.Property(x => x.StackTrace)
            .HasColumnType("text");

        builder.HasOne(x => x.OutboxMessage)
            .WithMany(om => om.OutboxPublishAttempts)
            .HasForeignKey(x => x.OutboxMessageId);

        builder.HasIndex(x => new { x.OutboxMessageId, x.AttemptNumber })
            .IsUnique();

        builder.ToTable("OutboxPublishAttempts");
    }
}
