using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Extensions;

public static class HasIdExtension
{
    public static async Task<T> ById<T, TKey>(this IQueryable<T> query, TKey id)
        where T : class, IHasId<TKey>
        where TKey : IEquatable<TKey> =>
        await query.ByProperty(x => x.Id, id);

    public static async Task<T> ByProperty<T, TProp>(this IQueryable<T> query, Expression<Func<T, TProp>> selector, TProp value)
        where T : class
        where TProp : IEquatable<TProp> =>
        await query.FirstOrDefaultAsync(
            Expression.Lambda<Func<T, bool>>(
                Expression.Equal(selector.Body, Expression.Constant(value)),
                selector.Parameters))
        ?? throw new ArgumentException($"2532. entity with field = '{value}' not found", nameof(TProp));
}
