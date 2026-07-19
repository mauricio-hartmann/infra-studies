using IS.Core.DomainObjects;

namespace IS.Ticket.API.Entities;

public class Customer : AuditedEntity, IAggregateRoot
{
    public string LegalName { get; init; }
    public string TradeName { get; init; }
    public string RegistrationNumber { get; init; }
}
