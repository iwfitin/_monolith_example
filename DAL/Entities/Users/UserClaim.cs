using Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities.Users;

public sealed class UserClaim : IdentityUserClaim<string>, IHasId<int>
{
    public AspNetUser User { get; set; }
}
