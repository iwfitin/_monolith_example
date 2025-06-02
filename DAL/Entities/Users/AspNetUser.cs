using Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities.Users;

public class AspNetUser : IdentityUser, IHasId<string>
{
    public ICollection<UserRole> Roles { get; set; }

    public ICollection<UserClaim> Claims { get; set; }
}
