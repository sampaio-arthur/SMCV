using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMCV.Application.DTOs.Users;
using SMCV.Features.Users.Commands.CreateUser;
using SMCV.Features.Users.Commands.DeleteUser;
using SMCV.Features.Users.Commands.UpdateUser;
using SMCV.Features.Users.Queries.GetAllUsers;
using SMCV.Features.Users.Queries.GetUserById;

namespace SMCV.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;
        return Ok(await _mediator.Send(new GetAllUsersQuery(pageNumber, pageSize)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (currentUserId != id) return Forbid();

        return Ok(await _mediator.Send(new GetUserByIdQuery(id)));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        Guid? keycloakId = null;
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is not null && Guid.TryParse(sub, out var parsed))
            keycloakId = parsed;

        var command = new CreateUserCommand(keycloakId, request.Name, request.Email);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (currentUserId != id) return Forbid();

        var result = await _mediator.Send(new UpdateUserCommand(id, request.Name, request.Email));
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (currentUserId != id) return Forbid();

        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}
