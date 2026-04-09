using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMCV.Application.DTOs.UserProfiles;
using SMCV.Features.UserProfiles.Commands.CreateUserProfile;
using SMCV.Features.UserProfiles.Commands.DeleteUserProfile;
using SMCV.Features.UserProfiles.Commands.UpdateUserProfile;
using SMCV.Features.UserProfiles.Queries.GetAllUserProfiles;
using SMCV.Features.UserProfiles.Queries.GetUserProfileById;
using SMCV.Features.UserProfiles.Queries.GetUserProfileByUserId;

namespace SMCV.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) =>
        Ok(await _mediator.Send(new GetAllUserProfilesQuery(pageNumber, pageSize)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        Ok(await _mediator.Send(new GetUserProfileByIdQuery(id)));

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId) =>
        Ok(await _mediator.Send(new GetUserProfileByUserIdQuery(userId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserProfileRequest request)
    {
        var command = new CreateUserProfileCommand(request.UserId, null);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/upload-resume")]
    public async Task<IActionResult> UploadResume(Guid id, IFormFile resumeFile)
    {
        if (resumeFile is null || resumeFile.Length == 0)
            return BadRequest(new { message = "Arquivo inválido." });

        var allowedExtensions = new[] { ".pdf" };
        var extension = Path.GetExtension(resumeFile.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Apenas arquivos .pdf são permitidos." });

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "resumes");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}_{resumeFile.FileName}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await resumeFile.CopyToAsync(stream);
        }

        var result = await _mediator.Send(new UpdateUserProfileCommand(id, filePath));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserProfileRequest request)
    {
        var result = await _mediator.Send(new UpdateUserProfileCommand(id, request.ResumeFilePath));
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserProfileCommand(id));
        return NoContent();
    }
}
