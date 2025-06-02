using System.Linq.Expressions;
using BLL.Base;
using DAL.EF;
using DAL.Entities.Users;
using DAL.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BLL.Users;

public class AspNetUserService : AspNetUserService<AspNetUser>
{
    public AspNetUserService(AppDbContext context) : base(context) { }

    public async Task<AspNetUser> ByName(string userName)
    {
        return await Context.Set<AspNetUser>().ByName(userName);
    }
}

public abstract class AspNetUserService<TU> : EntityService<TU, string> where TU : AspNetUser
{
    protected AspNetUserService(AppDbContext context) : base(context) { }

    protected override async Task BeforeAdd(TU entity)
    {
        await base.BeforeAdd(entity);
        entity.Id = Guid.NewGuid().ToString("N");
    }

    protected override void Transform(TU entity)
    {
        base.Transform(entity);
        entity.Email = entity.Email?.Trim();
        entity.UserName = entity.Email;
        entity.PhoneNumber = entity.PhoneNumber?.Trim();
        entity.NormalizedUserName = entity.UserName?.ToUpper();
        entity.NormalizedEmail = entity.Email?.ToUpper();
    }

    protected override async Task Check(TU entity)
    {
        await base.Check(entity);
        if (await Context.Set<TU>()
                .AnyAsync(x => x.Id != entity.Id && x.UserName == entity.UserName))
            throw new ArgumentException($"2502. user with name {entity.UserName} already exists");
    }

    public override async Task Delete(string id)
    {
        var entity = await Context.Set<TU>()
            .Include(x => x.Roles)
            .Include(x => x.Claims)
            .ById(id);
        
        Context.Set<TU>().Remove(entity);

        await SaveChanges();
    }

    public async Task<ICollection<string>> RoleNames(string id)
    {
        return await Roles(id, x => x.Role.Name);
    }

    private async Task<ICollection<string>> Roles(string id, Expression<Func<UserRole, string>> selector)
    {
        return await Context.Set<UserRole>()
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .Select(selector)
            .ToListAsync();
    }
}
