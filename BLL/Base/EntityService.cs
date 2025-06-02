using System.Linq.Expressions;
using Common.Extensions;
using Common.Interfaces;
using DAL.EF;
using DAL.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BLL.Base;

public abstract class EntityService<TE, TK> : ContextHasService
    where TE : class, IHasId<TK>
    where TK : IEquatable<TK>
{
    protected EntityService(AppDbContext context) : base(context) { }

    public virtual async Task<TE> Add<T>(T dto, TypeAdapterConfig cnf = null)
    {
        cnf ??= TypeAdapterConfig.GlobalSettings;
        var entity = dto?.Adapt<TE>(cnf) ?? throw new ArgumentException($"2518. {typeof(T)} is null");
        await BeforeAdd(entity);
        Transform(entity);
        await Check(entity);
        await Context.Set<TE>().AddAsync(entity);
        await SaveChanges();

        return entity;
    }

    protected virtual Task BeforeAdd(TE entity)
    {
        return Task.CompletedTask;
    }

    protected virtual void Transform(TE entity) { }

    protected virtual Task Check(TE entity)
    {
        return Task.CompletedTask;
    }

    public virtual async Task<IEnumerable<T>> List<T>(TypeAdapterConfig cnf = null)
    {
        return await List<T>(Context.Set<TE>(), cnf);
    }

    public async Task<IEnumerable<T>> List<T>(IQueryable<TE> query, TypeAdapterConfig cnf = null)
    {
        return await List<T>(query, x => x.OrderByDescending(x => x.Id), cnf);
    }

    public virtual async Task<IEnumerable<T>> List<T>(Func<IQueryable<TE>, IOrderedQueryable<TE>> sorting,
        TypeAdapterConfig cnf = null)
    {
        return await List<T>(Context.Set<TE>(), sorting, cnf);
    }

    public async Task<IEnumerable<T>> List<T>(IQueryable<TE> query, Func<IQueryable<TE>, IOrderedQueryable<TE>> sorting,
        TypeAdapterConfig cnf = null)
    {
        return await sorting(query)
            .AsNoTracking()
            .ProjectToType<T>(cnf)
            .ToListAsync();
    }

    public virtual async Task<T> ById<T>(TK id) where T : class, IHasId<TK>
    {
        return await ById<T>(id, null);
    }

    public virtual async Task<T> ById<T>(TK id, TypeAdapterConfig cnf) where T : class, IHasId<TK>
    {
        return await Context.Set<TE>()
            .AsNoTracking()
            .ProjectToType<T>(cnf)
            .ById(id);
    }

    public async Task<T> Select<T>(TK id, Expression<Func<TE, T>> selector, bool isThrow = true)
    {
        return await Select(x => x.Id.Equals(id), selector, isThrow);
    }

    public async Task<T> Select<T>(Expression<Func<TE, bool>> predicate, Expression<Func<TE, T>> selector,
        bool isThrow = true)
    {
        var query = Context.Set<TE>()
            .AsNoTracking()
            .Where(predicate)
            .Select(selector);

        if (isThrow && !await query.AnyAsync())
            throw new ArgumentException($"2520. {typeof(TE).Name} not found, predicate = {predicate}");

        return await query.FirstAsync();
    }

    public virtual async Task Edit<T>(T dto) where T : class, IHasId<TK>
    {
        await Edit(dto, null);
    }

    public virtual async Task Edit<T>(T dto, TypeAdapterConfig cnf) where T : class, IHasId<TK>
    {
        var entity = await Context.Set<TE>().ById(dto.Id);
        dto.Adapt(entity, cnf ?? TypeAdapterConfig.GlobalSettings);
        Transform(entity);
        await Check(entity);

        await SaveChanges();
    }

    public virtual async Task Delete(TK id)
    {
        Context.Set<TE>().Remove(await ById<TE>(id));
    }

    protected void Edit<TL, TLk>(ICollection<TL> was, ICollection<TL> become,
        Func<TL, TL, bool> equal, TypeAdapterConfig cnf)
        where TL : IHasId<TLk>
    {
        var (add, del, edt) = was.Merge(become, equal, x => x.Id);

        was.AddRange(add);
        was.RemoveRange(del);
        foreach (var (x, t) in edt)
            t.Adapt(x, cnf);
    }

    public async Task SingleUpdate(TK id, Expression<Func<SetPropertyCalls<TE>, SetPropertyCalls<TE>>> setPropertyCalls)
    {
        var count = await Context.Set<TE>()
            .Where(x => x.Id.Equals(id))
            .ExecuteUpdateAsync(setPropertyCalls);
        CheckAffectedCount(id, count, "updated");
    }

    private void CheckAffectedCount(TK id, int count, string operation)
    {
        if (count == 0)
            throw new ArgumentException($"2588. entity {typeof(TE)} with id = {id} not found");

        if (count != 1)
            throw new ArgumentException($"2587. there are more than one entities have been {operation}");
    }

    public virtual async Task SingleDelete(TK id)
    {
        var count = await Context.Set<TE>()
            .Where(x => x.Id.Equals(id))
            .ExecuteDeleteAsync();

        CheckAffectedCount(id, count, "deleted");
    }
}
