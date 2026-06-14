using IS.Core.DomainObjects;

namespace IS.Customers.API.Entities
{
    public class Customer : AuditedEntity, IAggregateRoot
    {
        public string LegalName { get; set; }
        public string NormalizedLegalName { get; set; }
        public string TradeName { get; set; }
        public string NormalizedTradeName { get; set; }
        public string RegistrationNumber { get; init; }
        public string Email { get; set; }
        public string MainPhone { get; set; }
        public string SecondaryPhone { get; set; }
        public string SiteUrl { get; set; }
        public string MainContactName { get; set; }
        public ICollection<Address> Addresses { get; init; }

        public Customer(string legalName, string tradeName, string registrationNumber) : base()
        {
            Addresses = [];
            LegalName = legalName;
            TradeName = tradeName;
            RegistrationNumber = registrationNumber;
        }

        public void AddAddress(Address address, bool isMainAddress)
        {
            if (isMainAddress)
            {
                address.IsMainAddress = true;

                foreach (var existingAddress in Addresses)
                    existingAddress.IsMainAddress = false;
            }

            Addresses.Add(address);
        }
    }
}
