using AutoFixture;
using IS.Core.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IS.Core.Tests.Data.Extensions
{
    public class IQueryableExtensionsTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public async Task ToPagedResultAsync_ShouldReturnRequestedPageAndPaginationMetadata()
        {
            await using TestDbContext dbContext = CreateDbContext();
            List<TestEntity> entities = CreateEntities(11);

            dbContext.Entities.AddRange(entities);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.Entities
                .OrderBy(x => x.Id)
                .ToPagedResultAsync(pageNumber: 2, pageSize: 5);

            Assert.Equal(2, result.PageNumber);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(11, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
            Assert.True(result.HasNext);
            Assert.Equal([6, 7, 8, 9, 10], result.Data.Select(x => x.Id));
        }

        [Fact]
        public async Task ToPagedResultAsync_ShouldReturnEmptyData_WhenRequestedPageIsAfterLastPage()
        {
            await using TestDbContext dbContext = CreateDbContext();
            List<TestEntity> entities = CreateEntities(3);

            dbContext.Entities.AddRange(entities);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.Entities
                .OrderBy(x => x.Id)
                .ToPagedResultAsync(pageNumber: 2, pageSize: 5);

            Assert.Empty(result.Data);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
            Assert.False(result.HasNext);
        }

        private TestDbContext CreateDbContext()
        {
            DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(_fixture.Create<string>())
                .Options;

            return new TestDbContext(options);
        }

        private static List<TestEntity> CreateEntities(int count)
        {
            return Enumerable.Range(1, count)
                .Select(id => new TestEntity
                {
                    Id = id,
                    Name = $"Entity {id}"
                })
                .ToList();
        }

        private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
        {
            public DbSet<TestEntity> Entities => Set<TestEntity>();
        }

        private sealed class TestEntity
        {
            public int Id { get; init; }
            public string Name { get; init; } = string.Empty;
        }
    }
}
