using Microsoft.AspNetCore.Mvc;
using SMCV.Application.DTOs.Auth;
using SMCV.Application.Interfaces;
using SMCV.Domain.Entities;

namespace SMCV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            return BadRequest(new { message = "Email já cadastrado." });

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _userRepository.AddAsync(user);

        HttpContext.Session.SetString("userId", user.Id.ToString());

        return Created("", new { id = user.Id, name = user.Name, email = user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new { message = "Email ou senha inválidos." });

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Email ou senha inválidos." });

        HttpContext.Session.SetString("userId", user.Id.ToString());

        return Ok(new { id = user.Id, name = user.Name, email = user.Email });
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

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user is null) return Unauthorized();

        return Ok(new { id = user.Id, name = user.Name, email = user.Email });
    }
}
