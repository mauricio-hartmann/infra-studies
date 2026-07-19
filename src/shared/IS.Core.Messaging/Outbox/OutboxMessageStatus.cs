namespace IS.Core.Messaging.Outbox;

public enum OutboxMessageStatus
{
    Pending = 1,
    Publishing = 2,
    Published = 3,
    Failed = 4
}
