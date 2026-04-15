using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SMCV.Application.DTOs.UserProfiles;
using SMCV.Application.Interfaces;
using SMCV.Features.UserProfiles.Commands.CreateUserProfile;
using SMCV.Features.UserProfiles.Commands.DeleteUserProfile;
using SMCV.Features.UserProfiles.Commands.UpdateUserProfile;
using SMCV.Features.UserProfiles.Commands.UploadResume;
using SMCV.Features.UserProfiles.Queries.GetAllUserProfiles;
using SMCV.Features.UserProfiles.Queries.GetUserProfileById;
using SMCV.Features.UserProfiles.Queries.GetUserProfileByUserId;

namespace SMCV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfilesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly long _maxResumeFileSizeBytes;

    public UserProfilesController(
        IMediator mediator,
        IUserProfileRepository userProfileRepository,
        IOptions<FormOptions> formOptions)
    {
        _mediator = mediator;
        _userProfileRepository = userProfileRepository;
        _maxResumeFileSizeBytes = formOptions.Value.MultipartBodyLengthLimit;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;
        return Ok(await _mediator.Send(new GetAllUserProfilesQuery(pageNumber, pageSize)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _mediator.Send(new GetUserProfileByIdQuery(id)));
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId) =>
        Ok(await _mediator.Send(new GetUserProfileByUserIdQuery(userId)));

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var userId = HttpContext.Session.GetString("userId");
        if (userId is null) return Unauthorized();

        var command = new CreateUserProfileCommand(Guid.Parse(userId), null);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/upload-resume")]
    public async Task<IActionResult> UploadResume(Guid id, IFormFile resumeFile)
    {
        if (resumeFile is null || resumeFile.Length == 0)
            return BadRequest(new { message = "Arquivo inválido." });

        if (resumeFile.Length > _maxResumeFileSizeBytes)
            return StatusCode(StatusCodes.Status413PayloadTooLarge, new { error = "Arquivo muito grande. Envie um arquivo menor." });

        var allowedExtensions = new[] { ".pdf" };
        var extension = Path.GetExtension(resumeFile.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Apenas arquivos .pdf são permitidos." });

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "resumes");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(resumeFile.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await resumeFile.CopyToAsync(stream);
        }

        var result = await _mediator.Send(new UploadResumeCommand(id, filePath));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserProfileRequest request)
    {
        var result = await _mediator.Send(new UpdateUserProfileCommand(id));
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteUserProfileCommand(id));
        return Ok(result);
    }
}
