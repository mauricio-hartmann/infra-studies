namespace IS.Customers.API.Features.GetCustomerById
{
    public record CustomerDTO
    {
        public Guid Id { get; init; }
        public string LegalName { get; init; }
        public string TradeName { get; init; }
        public string RegistrationNumber { get; init; }
        public string Email { get; init; }
        public string MainPhone { get; init; }
        public string SecondaryPhone { get; init; }
        public string SiteUrl { get; init; }
        public string MainContactName { get; init; }
        public IEnumerable<AddressDTO> Addresses { get; init; }
    }

    public record AddressDTO
    {
        public Guid Id { get; init; }
        public string Street { get; init; }
        public string Number { get; init; }
        public string AddressComplement { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string Country { get; init; }
        public bool IsMainAddress { get; init; }
    }
}
