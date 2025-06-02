using API.Infrastructure.Middleware;
using BLL.Users;
using Common.Enums;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Users;

[Route("api/users")]
[AuthorizeRoles(RoleType.Admin)]
public class UsersController : IdentityController
{
    private UserService Service { get; }

    public UsersController(UserService service)
    {
        Service = service;
    }

    /// <summary>
    /// about me
    /// </summary>
    /// <response code="400">not found by identifier</response>
    /// <response code="401">unauthorized</response>
    /// <response code="403">forbidden</response>
    /// <response code="500">uncaught, unknown error</response>
    [HttpGet]
    [Route("about-me")]
    [ProducesResponseType(typeof(UserDto.AboutMe), StatusCodes.Status200OK)]
    public async Task<UserDto.AboutMe> AboutMe([FromServices] AspNetUserService service)
    {
        return await service.ById<UserDto.AboutMe>(UserId());
    }

    /// <summary>
    /// add
    /// </summary>
    /// <param name="dto">payload</param>
    /// <response code="400">payload error</response>
    /// <response code="401">unauthorized</response>
    /// <response code="403">forbidden</response>
    /// <response code="500">uncaught, unknown error</response>
    [HttpPost]
    [Route("")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<string> Add(UserDto.Add dto)
    {
        return await Service.Add(dto);
    }

    /// <summary>
    /// get list
    /// </summary>
    /// <response code="400">not found by identifier</response>
    /// <response code="401">unauthorized</response>
    /// <response code="403">forbidden</response>
    /// <response code="500">uncaught, unknown error</response>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(IEnumerable<UserDto.List>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<UserDto.List>> List()
    {
        return await Service.List<UserDto.List>();
    }

    /// <summary>
    /// get by identifier
    /// </summary>
    /// <param name="id">identifier</param>
    /// <response code="400">not found by identifier</response>
    /// <response code="401">authorization required</response>
    /// <response code="403">forbidden</response>
    /// <response code="500">uncaught, unknown error</response>
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(UserDto.ByHasId), StatusCodes.Status200OK)]
    public async Task<UserDto.ByHasId> ById(string id)
    {
        return await Service.ById<UserDto.ByHasId>(id);
    }

    /// <summary>
    /// edit
    /// </summary>
    /// <param name="dto">payload</param>
    /// <response code="400">payload error</response>
    /// <response code="401">unauthorized</response>
    /// <response code="403">forbidden</response>
    /// <response code="500">uncaught, unknown error</response>
    [HttpPut]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task Edit(UserDto.Edit dto)
    {
        await Service.Edit(dto);
    }

    /// <summary>
    /// delete
    /// </summary>
    /// <param name="id">identifier</param>
    /// <response code="400">not found by identifier</response>
    /// <response code="401">unauthorized</response>
    /// <response code="403">forbidden</response>
    /// <response code="403">forbidden</response>
    /// <response code="500">uncaught, unknown error</response>
    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task Delete(string id)
    {
        await Service.Delete(id);
    }
}
