namespace IS.Core.Messaging.Outbox;

public sealed class OutboxPublishAttempt
{
    public Guid Id { get; set; }
    public Guid OutboxMessageId { get; set; }
    public int AttemptNumber { get; set; }
    public OutboxPublishAttemptStatus Status { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string PublishingBy { get; set; }
    public string ErrorMessage { get; set; }
    public string ExceptionType { get; set; }
    public string StackTrace { get; set; }

    // EF Relations
    public OutboxMessage OutboxMessage { get; set; }

    public OutboxPublishAttempt()
    {
        Id = Guid.NewGuid();
    }
}
