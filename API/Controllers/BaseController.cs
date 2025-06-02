using DTO.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ProducesResponseType(typeof(BadRequestDto), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(BadRequestDto), StatusCodes.Status500InternalServerError)]
public abstract class BaseController : ControllerBase { }
