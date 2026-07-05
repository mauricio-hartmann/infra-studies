using IS.Core.DomainObjects;
using IS.Core.Extensions;

namespace IS.Customers.API.Entities
{
    public class Customer : AuditedEntity, IAggregateRoot
    {
        public string LegalName { get; set; }
        public string NormalizedLegalName { get; set; }
        public string TradeName { get; set; }
        public string NormalizedTradeName { get; set; }
        public string RegistrationNumber { get; init; }
        public string? Email { get; set; }
        public string MainPhone { get; set; }
        public string? SecondaryPhone { get; set; }
        public string? SiteUrl { get; set; }
        public string? MainContactName { get; set; }
        public ICollection<Address> Addresses { get; init; }

        public Customer(string legalName, string tradeName, string registrationNumber) : base()
        {
            Addresses = [];
            LegalName = legalName;
            NormalizedLegalName = legalName.NormalizeToUpper();
            TradeName = tradeName;
            NormalizedTradeName = tradeName.NormalizeToUpper();
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

        public void Update(string legalName, string tradeName, string? email, string mainPhone, string? secondaryPhone, string? siteUrl, string mainContactName)
        {
            LegalName = legalName;
            NormalizedLegalName = legalName.NormalizeToUpper();
            TradeName = tradeName;
            NormalizedTradeName = tradeName.NormalizeToUpper();
            Email = email;
            MainPhone = mainPhone;
            SecondaryPhone = secondaryPhone;
            SiteUrl = siteUrl;
            MainContactName = mainContactName;
        }

        public bool DeleteAddress(Guid addressId, Guid? newMainAddressId)
        {
            Address? address = Addresses.FirstOrDefault(a => a.Id == addressId);

            if (address is null)
                return false;

            bool wasMainAddress = address.IsMainAddress;
            address.IsMainAddress = false;
            address.Delete();

            if (wasMainAddress && newMainAddressId.HasValue)
            {
                Address? newMain = Addresses.FirstOrDefault(a => a.Id == newMainAddressId.Value);
                if (newMain is not null)
                    newMain.IsMainAddress = true;
            }

            return true;
        }
    }
}
