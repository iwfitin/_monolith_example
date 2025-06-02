using Common.Enums;
using Common.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace API.Infrastructure.Middleware;

internal class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params RoleType[] allowedRoles)
    {
        Roles = string.Join(",", allowedRoles.Select(x => x.Description()));
    }
}
