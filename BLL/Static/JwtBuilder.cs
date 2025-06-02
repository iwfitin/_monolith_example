using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Common.Extensions;
using DTO.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Static;

public static class JwtBuilder
{
    public static TokenValidationParameters Parameters(IConfiguration configuration)
    {
        var settings = configuration.GetSection(nameof(AuthDto.Jwt)).Get<AuthDto.Jwt>();
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = settings.Issuer,
            ValidAudience = settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(settings.Key.ToBytes()),
            ClockSkew = TimeSpan.Zero,
        };
    }

    public static string SecurityToken(IEnumerable<Claim> claims, string issuer, string audience,
        DateTime notBefore, DateTime expire, string key)
    {
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: notBefore,
            expires: expire,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key.ToBytes()),
                SecurityAlgorithms.HmacSha256Signature));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string Bearer()
    {
        return nameof(Bearer).ToLower();
    }

    public static string Authorization()
    {
        return nameof(Authorization);
    }
}
