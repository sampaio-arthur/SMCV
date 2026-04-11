using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMCV.Features.Auth.Commands.LoginUser;
using SMCV.Features.Auth.Commands.RegisterUser;
using SMCV.Features.Auth.Queries.GetCurrentUser;
using SMCV.Application.DTOs.Auth;

namespace SMCV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request.Name, request.Email, request.Password));
        HttpContext.Session.SetString("userId", result.Id.ToString());
        return Created("", result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request.Email, request.Password));
        HttpContext.Session.SetString("userId", result.Id.ToString());
        return Ok(result);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = HttpContext.Session.GetString("userId");
        if (userId is null) return Unauthorized();

        var result = await _mediator.Send(new GetCurrentUserQuery(Guid.Parse(userId)));
        return Ok(result);
    }
}
