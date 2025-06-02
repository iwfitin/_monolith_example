using Common.Extensions;
using Common.Interfaces;
using DAL.EF;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BLL.Base;

public abstract class EntityUltimateService<TE, TK> : EntityService<TE, TK>
    where TE : class, IHasId<TK>
    where TK : IEquatable<TK>
{
    protected EntityUltimateService(AppDbContext context) : base(context) { }

    public virtual async Task<(IEnumerable<T> Data, int Total)> Page<T>(int number, int size)
    {
        return (await Context.Set<TE>()
            .Page(number, size)
            .ProjectToType<T>()
            .ToListAsync(), await Total(Context.Set<TE>(), size));
    }

    protected async Task<int> Total(IQueryable<TE> query, int size)
    {
        var count = await query
            .CountAsync();
        return count == 0
            ? 1
            : count / size + (count % size > 0 ? 1 : 0);
    }

    public async Task RemoveRange(IEnumerable<TK> dto)
    {
        await Context.Set<TE>()
            .Where(x => dto.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    protected async Task Repackage<TL, TLk>(ICollection<TL> was, ICollection<TL> become, string collectionName, string errorNumber)
        where TL : class, IHasId<TLk>
    {
        become = await Reattach<TL, TLk>(become, collectionName, errorNumber);
        var (add, del) = was.Merge(become, x => x.Id);
        was.AddRange(add);
        was.RemoveRange(del);
    }

    protected async Task<ICollection<TL>> Reattach<TL, TLk>(ICollection<TL> refs, string collectionName, string errorNumber)
        where TL : class, IHasId<TLk>
    {
        var ids = refs
            .Select(x => x.Id);
        var lst = await Context.Set<TL>()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();

        if (lst.Count != ids.Count())
            throw new ArgumentException($"{errorNumber} {typeof(TL)} does not exit", collectionName);

        return lst;
    }

    public async Task<IReadOnlyDictionary<TKey, TElement>> ToDict<TKey, TElement>(Func<TE, TKey> keySelector,
        Func<TE, TElement> elementSelector)
    {
        return await Context.Set<TE>()
            .ToDictionaryAsync(keySelector, elementSelector);
    }
}
