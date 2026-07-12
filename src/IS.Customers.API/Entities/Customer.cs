using IS.Core.Communication;
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

        public void Update(string legalName, string tradeName, string email, string mainPhone, string secondaryPhone, string siteUrl, string mainContactName)
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

        public BaseResult<bool> UpdateAddress(Guid addressId, string street, string number, string addressComplement, string city, string state, string country, bool isMainAddress)
        {
            Address address = Addresses.FirstOrDefault(a => a.Id == addressId);

            if (address is null)
                return BaseResult<bool>.Failure("Address does not exist or does not belong to this customer!");

            if (!isMainAddress && Addresses.Count(a => a.Id != addressId && a.IsMainAddress) != 1)
                return BaseResult<bool>.Failure("Customer must have one main address!");

            address.Update(street, number, addressComplement, city, state, country);
            address.IsMainAddress = isMainAddress;

            if (isMainAddress)
                SetMainAddress(address);

            if (!HasExactlyOneMainAddress())
                return BaseResult<bool>.Failure("Customer must have exactly one main address!");

            return BaseResult<bool>.Success(true);
        }

        public bool DeleteAddress(Guid addressId, Guid? newMainAddressId)
        {
            Address address = Addresses.FirstOrDefault(a => a.Id == addressId);

            if (address is null)
                return false;

            bool wasMainAddress = address.IsMainAddress;
            Guid newMainAddressIdValue = Guid.Empty;

            if (wasMainAddress)
            {
                if (!newMainAddressId.HasValue)
                    return false;

                newMainAddressIdValue = newMainAddressId.GetValueOrDefault();

                if (newMainAddressIdValue == addressId)
                    return false;

                if (Addresses.All(a => a.Id != newMainAddressIdValue))
                    return false;
            }
            else if (Addresses.Count(a => a.Id != addressId && a.IsMainAddress) != 1)
            {
                return false;
            }

            address.IsMainAddress = false;
            address.Delete();

            if (wasMainAddress)
            {
                Address newMain = Addresses.First(a => a.Id == newMainAddressIdValue);
                SetMainAddress(newMain);
            }

            return HasExactlyOneMainAddress();
        }

        private void SetMainAddress(Address mainAddress)
        {
            foreach (var existingAddress in Addresses)
                existingAddress.IsMainAddress = existingAddress.Id == mainAddress.Id;
        }

        private bool HasExactlyOneMainAddress()
        {
            return Addresses.Count(a => !a.DateDeleted.HasValue && a.IsMainAddress) == 1;
        }
    }
}
