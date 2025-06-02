using Microsoft.AspNetCore.Identity;

namespace DAL.Entities.Users;

public sealed class UserRole : IdentityUserRole<string>
{
    public AspNetUser User { get; set; }

    public Role Role { get; set; }
}
