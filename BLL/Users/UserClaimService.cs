using BLL.Base;
using Common.Extensions;
using DAL.EF;
using DAL.Entities.Users;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BLL.Users;

public sealed class UserClaimService : EntityService<UserClaim, int>
{
    public UserClaimService(AppDbContext context) : base(context) { }

    protected override async Task Check(UserClaim entity)
    {
        await base.Check(entity);
        if (await Context.Set<UserClaim>()
                .AnyAsync(x => x.Id != entity.Id && x.UserId == entity.UserId && x.ClaimValue == entity.ClaimValue))
            throw new ArgumentException($"2572. claim {entity.ClaimValue} already added");
    }

    public async Task<IEnumerable<T>> List<T>(string userId, TypeAdapterConfig cnf = null)
    {
        var query = Context.Set<UserClaim>().AsQueryable();
        if (userId.IsNotNullOrEmpty())
            query = query.Where(x => x.UserId == userId);

        return await List<T>(query, x => x.OrderBy(x => x.ClaimValue), cnf);
    }

    public async Task Add(IEnumerable<(string UserId, string Type, string Value)> payload)
    {
        await Context.Set<UserClaim>().AddRangeAsync(payload
            .Select(x => new UserClaim
            {
                UserId = x.UserId,
                ClaimType = x.Type,
                ClaimValue = x.Value,
            }));

        await SaveChanges();
    }

    public async Task Delete(IEnumerable<int> payload)
    {
        await Context.Set<UserClaim>()
            .Where(x => payload.Contains(x.Id))
            .ExecuteDeleteAsync();
    }
}
