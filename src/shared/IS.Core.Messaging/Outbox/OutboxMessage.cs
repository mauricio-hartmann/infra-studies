using System.Text.Json;

namespace IS.Core.Messaging.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string EventType { get; init; }
    public string Payload { get; init; }
    public OutboxMessageStatus Status { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public DateTime? PublishingStartedAtUtc { get; set; }
    public DateTime? PublishingExpiresAtUtc { get; set; }
    public DateTime? NextAttemptAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public int AttemptCount { get; set; }
    public string PublishingBy { get; set; }
    public Guid AggregateId { get; set; }

    // EF Relations
    public ICollection<OutboxPublishAttempt> OutboxPublishAttempts { get; init; }

    public OutboxMessage(Guid aggregateId)
    {
        Id = Guid.NewGuid();
        Status = OutboxMessageStatus.Pending;
        OccurredAtUtc = DateTime.UtcNow;
        NextAttemptAtUtc = DateTime.UtcNow;
        AttemptCount = 0;
        AggregateId = aggregateId;
        OutboxPublishAttempts = [];
    }


    public OutboxMessage(Guid aggregateId, string eventType, string payload) : this(aggregateId)
    {
        EventType = eventType;
        Payload = payload;
    }

    public OutboxMessage(Guid aggregateId, string eventType, object payload) : this(aggregateId)
    {
        EventType = eventType;
        Payload = JsonSerializer.Serialize(payload);
    }
}
