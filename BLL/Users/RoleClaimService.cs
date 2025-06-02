using BLL.Base;
using DAL.EF;
using DAL.Entities.Users;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BLL.Users;

public sealed class RoleClaimService : EntityService<RoleClaim, int>
{
    public RoleClaimService(AppDbContext context) : base(context) { }

    protected override async Task Check(RoleClaim entity)
    {
        await base.Check(entity);
        if (await Context.Set<RoleClaim>()
                .AnyAsync(x => x.Id != entity.Id && x.RoleId == entity.RoleId && x.ClaimValue == entity.ClaimValue))
            throw new ArgumentException($"2571. claim {entity.ClaimValue} already added");
    }

    public async Task<IEnumerable<T>> List<T>(string roleId, TypeAdapterConfig cnf = null)
    {
        return await List<T>(new[] { roleId, }, cnf);
    }

    public async Task<IEnumerable<T>> List<T>(ICollection<string> roles, TypeAdapterConfig cnf = null)
    {
        var query = Context.Set<RoleClaim>().AsQueryable();
        if (roles.Any())
            query = query.Where(x => x.RoleId == roles.First());
        query = roles.Skip(1).Aggregate(query, (x, roleId) => x.Union(x.Where(x => x.RoleId == roleId)));

        return await List<T>(query, x => x.OrderBy(x => x.ClaimValue), cnf);
    }
}
