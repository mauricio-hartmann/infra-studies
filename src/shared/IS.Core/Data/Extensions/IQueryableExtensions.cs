using IS.Core.Data.Pagination;
using Microsoft.EntityFrameworkCore;

namespace IS.Core.Data.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> queryable, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            int count = await queryable.CountAsync(cancellationToken);
            List<T> data = await queryable.Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync(cancellationToken);

            return new PagedResult<T>(data, pageNumber, pageSize, count);
        }
    }
}
