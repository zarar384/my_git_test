using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace LeaveMeAloneFuncSkillForge.Functional
{
    public static class KeysetPaginationExtensions
    {
        // Keyset (seek) pagination for EF Core.
        // Uses a cursor(Keyset) instead of offset(Skip / Take) to ensure stable and efficient paging.
        public static async Task<KeysetPage<T, TKey>> ToKeysetPageAsync<T, TKey>(
            this IQueryable<T> query,
            Expression<Func<T, TKey>> keySelector, // Field used as a cursor (must be indexed)
            TKey? lastKey,                          // Cursor from the previous page
            int pageSize,
            bool ascending = true,
            CancellationToken cancellationToken = default)
            where TKey : struct, IComparable<TKey>
        {
            // fallback page size
            if (pageSize <= 0)
                pageSize = 20;

            // apply keyset filter only when a cursor is provided
            if (lastKey.HasValue)
            {
                query = ascending
                    ? query.Where(BuildGreaterThan(keySelector, lastKey.Value))
                    : query.Where(BuildLessThan(keySelector, lastKey.Value));
            }

            // always order by the cursor field
            query = ascending
                ? query.OrderBy(keySelector)
                : query.OrderByDescending(keySelector);

            // fetch one extra item to check for next page to detect if the next page exists
            var pageQuery = query.Take(pageSize + 1);

            List<T> items;

            if(query is IAsyncEnumerable<T>)
            {
                items = await pageQuery.ToListAsync(cancellationToken);
            }
            else
            {
                items = pageQuery.ToList();
            }

            var hasNextPage = items.Count > pageSize;

            // remove the extra item before returning the result
            if (hasNextPage)
                items.RemoveAt(items.Count - 1);

            // the next cursor is the last item's key
            TKey? nextCursor = hasNextPage
                ? keySelector.Compile()(items[^1])
                : null;

            return new KeysetPage<T, TKey>(
                items,
                hasNextPage,
                nextCursor
            );
        }

        // Builds: entity => entity.Key > value
        private static Expression<Func<T, bool>> BuildGreaterThan<T, TKey>(
            Expression<Func<T, TKey>> keySelector,
            TKey value)
        {
            var parameter = keySelector.Parameters[0];
            var body = Expression.GreaterThan(
                keySelector.Body,
                Expression.Constant(value)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        // Builds: entity => entity.Key < value
        private static Expression<Func<T, bool>> BuildLessThan<T, TKey>(
            Expression<Func<T, TKey>> keySelector,
            TKey value)
        {
            var parameter = keySelector.Parameters[0];
            var body = Expression.LessThan(
                keySelector.Body,
                Expression.Constant(value)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
