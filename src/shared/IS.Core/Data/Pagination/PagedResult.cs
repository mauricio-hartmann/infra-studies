namespace IS.Core.Data.Pagination
{
    public record PagedResult<T>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public bool HasNext { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
        public IEnumerable<T> Data { get; init; }

        public PagedResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = TotalCount > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
            HasNext = PageNumber < TotalPages;
        }
    }
}
