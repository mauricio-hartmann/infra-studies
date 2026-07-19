namespace IS.Customers.API.Shared
{
    public static class CacheKeys
    {
        public static string Customer(Guid customerId) => $"cache:customer:{customerId}";
    }
}
