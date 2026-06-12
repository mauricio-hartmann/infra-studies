namespace IS.Core.DomainObjects
{
    public abstract class AuditedEntity : BaseEntity
    {
        public DateTime DateCreated { get; set; }
        public DateTime? DateDeleted { get; set; }

        protected AuditedEntity() : base()
        {
            DateCreated = DateTime.UtcNow;
        }

        public void Delete()
        {
            DateDeleted = DateTime.UtcNow;
        }
    }
}
