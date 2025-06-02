using BLL.Base;
using DAL.EF;
using DAL.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace BLL.Users;

public sealed class RoleService : EntityService<Role, string>
{
    public RoleService(AppDbContext context) : base(context) { }

    protected override async Task BeforeAdd(Role entity)
    {
        await base.BeforeAdd(entity);
        entity.Id = Guid.NewGuid().ToString("N");
    }

    protected override void Transform(Role entity)
    {
        base.Transform(entity);
        entity.NormalizedName = entity.Name?.ToUpper();
    }

    public async Task<IEnumerable<string>> Users(string id)
    {
        return await Context.Set<UserRole>()
            .Where(x => x.RoleId == id)
            .Select(x => x.UserId)
            .ToListAsync();
    }
}
