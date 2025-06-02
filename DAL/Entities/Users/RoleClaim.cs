using Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities.Users;

public sealed class RoleClaim : IdentityRoleClaim<string>, IHasId<int>
{
    public Role Role { get; set; }
}
