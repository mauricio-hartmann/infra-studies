using Microsoft.EntityFrameworkCore;

namespace IS.Core.Messaging.Outbox.Data;

public static class OutboxModelBuilderExtensions
{
    public static ModelBuilder AddOutbox(this ModelBuilder builder)
    {
        builder.ApplyConfiguration(new OutboxMessageMapping());
        builder.ApplyConfiguration(new OutboxPublishAttemptMapping());

        return builder;
    }
}
