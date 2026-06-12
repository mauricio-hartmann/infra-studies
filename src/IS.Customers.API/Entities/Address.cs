using IS.Core.DomainObjects;

namespace IS.Customers.API.Entities
{
    public class Address : AuditedEntity
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string AddressComplement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsMainAddress { get; set; }
        public Guid CustomerId { get; init; }
        public Customer Customer { get; init; }

        public Address(string street, string number, string addressComplement, string city, string state, string country, bool isMainAddress) : base()
        {
            Street = street;
            Number = number;
            AddressComplement = addressComplement;
            City = city;
            State = state;
            Country = country;
            IsMainAddress = isMainAddress;
        }

        public Address(string street, string number, string city, string state, string country, bool isMainAddress) : base()
        {
            Street = street;
            Number = number;
            City = city;
            State = state;
            Country = country;
            IsMainAddress = isMainAddress;
        }
    }
}
