using BLL.Users;
using Microsoft.AspNetCore.Mvc;
using DTO.Infrastructure;

namespace API.Controllers;

[Route("api/token")]
public class AuthController : BaseController
{
    private AuthService Service { get; }

    public AuthController(AuthService service)
    {
        Service = service;
    }

    /// <summary>
    /// get access and refresh token with login and pwd
    /// </summary>
    /// <param name="dto">login and pwd</param>
    /// <response code="400">payload error</response>
    /// <response code="500">uncaught, unknown error</response>
    [HttpPost]
    [Route("access")]
    [ProducesResponseType(typeof(AuthDto.Response), StatusCodes.Status200OK)]
    public async Task<AuthDto.Response> AccessToken(AuthDto.Login dto)
    {
        return await Service.AccessToken(dto);
    }

    /// <summary>
    /// get access and refresh token with old refresh token 
    /// </summary>
    /// <param name="dto">old refresh token</param>
    /// <response code="400">payload error</response>
    /// <response code="500">uncaught, unknown error</response>
    [HttpPost]
    [Route("refresh")]
    [ProducesResponseType(typeof(AuthDto.Response), StatusCodes.Status200OK)]
    public async Task<AuthDto.Response> RefreshToken(AuthDto.Refresh dto)
    {
        return await Service.RefreshToken(dto);
    }
}
