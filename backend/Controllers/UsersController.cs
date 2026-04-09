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
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) =>
        Ok(await _mediator.Send(new GetAllUsersQuery(pageNumber, pageSize)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        Ok(await _mediator.Send(new GetUserByIdQuery(id)));

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.Name, request.Email);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var result = await _mediator.Send(new UpdateUserCommand(id, request.Name, request.Email));
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}
