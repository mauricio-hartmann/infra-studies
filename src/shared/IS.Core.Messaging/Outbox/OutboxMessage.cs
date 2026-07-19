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

    public OutboxMessage()
    {
        Id = Guid.NewGuid();
        Status = OutboxMessageStatus.Pending;
        OccurredAtUtc = DateTime.UtcNow;
        NextAttemptAtUtc = DateTime.UtcNow;
        AttemptCount = 0;
        OutboxPublishAttempts = [];
    }


    public OutboxMessage(string eventType, string payload) : this()
    {
        EventType = eventType;
        Payload = payload;
    }
}
