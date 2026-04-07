using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMCV.Application.DTOs.Campaigns;
using SMCV.Features.Campaigns.Commands.CreateCampaign;
using SMCV.Features.Campaigns.Commands.DeleteCampaign;
using SMCV.Features.Campaigns.Commands.ExportContactsCsv;
using SMCV.Features.Campaigns.Commands.SendCampaignEmails;
using SMCV.Features.Campaigns.Commands.UpdateCampaign;
using SMCV.Features.Campaigns.Queries.GetAllCampaigns;
using SMCV.Features.Campaigns.Queries.GetCampaignById;

namespace SMCV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;

    private static readonly string[] AllowedExtensions = [".pdf", ".doc", ".docx"];

    public CampaignsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllCampaignsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCampaignByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateCampaignRequest request, IFormFile resumeFile)
    {
        if (resumeFile == null || resumeFile.Length == 0)
            return BadRequest(new { message = "Arquivo de currículo é obrigatório." });

        var extension = Path.GetExtension(resumeFile.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return BadRequest(new { message = "Apenas arquivos .pdf, .doc e .docx são permitidos." });

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "resumes");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}_{resumeFile.FileName}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await resumeFile.CopyToAsync(stream);
        }

        var command = new CreateCampaignCommand(
            request.Niche,
            request.Region,
            resumeFile.FileName,
            filePath,
            request.EmailSubject,
            request.EmailBody);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCampaignRequest request)
    {
        var command = new UpdateCampaignCommand(id, request.EmailSubject, request.EmailBody);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCampaignCommand(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/send")]
    public async Task<IActionResult> SendEmails(Guid id)
    {
        var count = await _mediator.Send(new SendCampaignEmailsCommand(id));
        return Ok(new { emailsSent = count });
    }

    [HttpGet("{id:guid}/export-csv")]
    public async Task<IActionResult> ExportCsv(Guid id)
    {
        var csvBytes = await _mediator.Send(new ExportContactsCsvCommand(id));
        return File(csvBytes, "text/csv", $"contacts_{id}.csv");
    }
}
