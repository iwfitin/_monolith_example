using DAL.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace DAL.Extensions;

public static class UserExtension
{
    public static async Task<T> ByName<T>(this IQueryable<T> query, string userName)
        where T : AspNetUser
    {
        return await query.FirstOrDefaultAsync(x => x.UserName == userName)
               ?? throw new ArgumentException($"2510. the user with name = '{userName}' is not found");
    }
}
