using AutoFixture;
using IS.Core.Data.Pagination;

namespace IS.Core.Tests.Data.Pagination
{
    public class PagedResultTests
    {
        private readonly Fixture _fixture = new();

        [Theory]
        [InlineData(1, 10, 0, 0, false)]
        [InlineData(1, 10, 10, 1, false)]
        [InlineData(1, 10, 11, 2, true)]
        [InlineData(2, 10, 11, 2, false)]
        [InlineData(3, 5, 12, 3, false)]
        public void Constructor_ShouldCalculatePaginationMetadata(
            int pageNumber,
            int pageSize,
            int totalCount,
            int expectedTotalPages,
            bool expectedHasNext)
        {
            List<int> data = _fixture.CreateMany<int>(Math.Min(pageSize, totalCount)).ToList();

            PagedResult<int> result = new(data, pageNumber, pageSize, totalCount);

            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(expectedTotalPages, result.TotalPages);
            Assert.Equal(expectedHasNext, result.HasNext);
            Assert.Equal(data, result.Data);
        }
    }
}
