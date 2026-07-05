namespace IS.Customers.API.Shared
{
    public static class ChacheKeys
    {
        public static string Customer(Guid customerId)
        {
            return $"-cache-customer-{customerId}";
        }
    }
}
