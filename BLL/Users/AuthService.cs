using System.Security.Claims;
using BLL.Static;
using Common.Extensions;
using DAL.Entities.Users;
using DTO.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BLL.Users;

public sealed class AuthService
{
    private static DateTime DayX { get; }

    static AuthService()
    {
        DayX = DateTime.UnixEpoch + TimeSpan.FromSeconds(int.MaxValue);
    }

    private AspNetUserService UserService { get; }

    private PasswordHasher<AspNetUser> PasswordHasher { get; }

    private AuthDto.Jwt Jwt { get; }

    public AuthService(AspNetUserService userService, PasswordHasher<AspNetUser> passwordHasher, IOptions<AuthDto.Jwt> jwt)
    {
        UserService = userService;
        PasswordHasher = passwordHasher;
        Jwt = jwt.Value;
    }

    public async Task<AuthDto.Response> AccessToken(AuthDto.Login dto)
    {
        var (user, claims, roleNames) = await UserClaimsRoleNames(dto.UserName, dto.Password);
        return BuildResponse(user, claims, roleNames);
    }

    private async Task<(AspNetUser User, IEnumerable<Claim> Claims, ICollection<string> RoleNames)>
        UserClaimsRoleNames(string username, string pwd)
    {
        var user = await UserService.ByName(username);
        switch (PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, pwd))
        {
            case PasswordVerificationResult.SuccessRehashNeeded:
                await UserService.SingleUpdate(user.Id, x =>
                    x.SetProperty(x => x.PasswordHash, x => PasswordHasher.HashPassword(null, pwd)));
                break;

            case PasswordVerificationResult.Failed:
                throw new ArgumentException("2511. wrong pwd", nameof(pwd));
        }

        var (claims, roleNames) = await ClaimsAndRoleNames(user);

        return (user, claims, roleNames);
    }

    private async Task<(IEnumerable<Claim> Claims, ICollection<string> RoleNames)> ClaimsAndRoleNames(AspNetUser user)
    {
        var roles = await UserService.RoleNames(user.Id);
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id),
        };
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

        return (claims, roles);
    }

    private AuthDto.Response BuildResponse(AspNetUser user, IEnumerable<Claim> claims, IEnumerable<string> roleNames, bool isEternal = false)
    {
        var (accessToken, accessExpireDate) = AccessToken(claims, isEternal);
        var (refreshToken, refreshExpireDate) = Refresh(user.UserName, isEternal);

        return new AuthDto.Response
        {
            AccessToken = accessToken,
            AccessTokenExpireDate = accessExpireDate,
            RefreshToken = refreshToken,
            RefreshTokenExpireDate = refreshExpireDate,
            UserName = user.UserName,
            RoleNames = roleNames,
        };
    }

    public (string Token, DateTime ExpireDate) Refresh(string userName, bool isEternal = false)
    {
        var now = DateTime.UtcNow;
        var refreshExpireDate = isEternal
            ? DayX
            : now + TimeSpan.FromMinutes(Jwt.RefreshTokenLifeTimeInMinutes);
        var token = new RefreshTokenDto
        {
            CreateDate = now,
            ExpireDate = refreshExpireDate,
            UserName = userName,
        }.ToJson().AesEncrypt();

        return (token, refreshExpireDate);
    }

    private (string AccessToken, DateTime Expires) AccessToken(IEnumerable<Claim> claims, bool isEternal = false)
    {
        var utcNow = DateTime.UtcNow;
        var expire = isEternal
            ? DayX
            : utcNow.Add(TimeSpan.FromMinutes(Jwt.AccessTokenLifeTimeInMinutes));
        var token = JwtBuilder.SecurityToken(claims, Jwt.Issuer, Jwt.Audience, utcNow, expire, Jwt.Key);

        return (token, expire);
    }

    public async Task<AuthDto.Response> RefreshToken(AuthDto.Refresh dto)
    {
        if (!dto.RefreshToken.IsBase64Str())
            throw new ArgumentException("2514. incorrect token", nameof(dto.RefreshToken));

        var refreshToken = dto.RefreshToken.AesDecrypt().FromJson<RefreshTokenDto>();
        var isEternal = refreshToken.ExpireDate == DayX;
        if (!isEternal && refreshToken.ExpireDate <= DateTime.UtcNow)
            throw new ArgumentException("2503. the refresh token has expired");

        var user = await UserService.ByName(refreshToken.UserName);
        var (claims, roleNames) = await ClaimsAndRoleNames(user);

        return BuildResponse(user, claims, roleNames, isEternal);
    }
}
