using System.Security.Claims;

namespace API.Controllers.Users;

public abstract class IdentityController : BaseController
{
    protected string UserId()
    {
        return HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
