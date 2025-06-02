using System.ComponentModel.DataAnnotations;

namespace DTO.Infrastructure;

public static class AuthDto
{
    public sealed record Login
    {
        [Required]
        public string UserName { get; init; }

        [Required]
        public string Password { get; init; }
    }

    public sealed record Refresh
    {
        [Required]
        public string RefreshToken { get; init; }
    }

    public sealed record Response
    {
        public string AccessToken { get; init; }

        public DateTime AccessTokenExpireDate { get; init; }

        public string RefreshToken { get; init; }

        public DateTime RefreshTokenExpireDate { get; init; }

        public string UserName { get; init; }

        public IEnumerable<string> RoleNames { get; init; }
    }

    public sealed record Jwt
    {
        public string Key { get; init; }

        public string Issuer { get; init; }

        public string Audience { get; init; }

        public int AccessTokenLifeTimeInMinutes { get; init; }

        public int RefreshTokenLifeTimeInMinutes { get; init; }
    }
}
