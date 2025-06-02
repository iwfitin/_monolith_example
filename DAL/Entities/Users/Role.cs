using Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities.Users;

public sealed class Role : IdentityRole, IHasId<string>
{
    public ICollection<UserRole> Users { get; set; }

    public ICollection<RoleClaim> Claims { get; set; }
}
