namespace IS.Core.Messaging.Outbox;

public enum OutboxPublishAttemptStatus
{
    Started = 1,
    Succeeded = 2,
    Failed = 3,
    Expired = 4
}
