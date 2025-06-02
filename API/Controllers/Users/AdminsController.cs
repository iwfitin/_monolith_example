using API.Infrastructure.Middleware;
using BLL.Users;
using Common.Enums;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Users;

[Route("api/admins")]
[AuthorizeRoles(RoleType.Admin)]
public class AdminsController : IdentityController
{
    private AdminService Service { get; }

    public AdminsController(AdminService service)
    {
        Service = service;
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
    public async Task<string> Add(AdminDto.Add dto)
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
    [ProducesResponseType(typeof(IEnumerable<AdminDto.List>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<AdminDto.List>> List()
    {
        return await Service.List<AdminDto.List>();
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
    [ProducesResponseType(typeof(AdminDto.ByHasId), StatusCodes.Status200OK)]
    public async Task<AdminDto.ByHasId> ById(string id)
    {
        return await Service.ById<AdminDto.ByHasId>(id);
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
    public async Task Edit(AdminDto.Edit dto)
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
