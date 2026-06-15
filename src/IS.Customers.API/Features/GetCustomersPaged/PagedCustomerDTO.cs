namespace IS.Customers.API.Features.GetCustomersPaged
{
    public record PagedCustomerDTO
    {
        public Guid Id { get; init; }
        public string LegalName { get; init; }
        public string TradeName { get; init; }
        public string RegistrationNumber { get; init; }
    }
}
