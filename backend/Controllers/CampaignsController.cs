using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMCV.Application.DTOs;
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
[Authorize]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CampaignResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllCampaignsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CampaignDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCampaignByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CampaignResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCampaignRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new CreateCampaignCommand(
            userId,
            request.Name,
            request.Niche,
            request.Region,
            request.EmailSubject,
            request.EmailBody);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CampaignResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCampaignRequest request)
    {
        var command = new UpdateCampaignCommand(id, request.Name, request.EmailSubject, request.EmailBody);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCampaignCommand(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendEmails(Guid id)
    {
        var count = await _mediator.Send(new SendCampaignEmailsCommand(id));
        return Ok(new { emailsSent = count });
    }

    [HttpGet("{id:guid}/export-csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportCsv(Guid id)
    {
        var csvBytes = await _mediator.Send(new ExportContactsCsvCommand(id));
        return File(csvBytes, "text/csv", $"contacts_{id}.csv");
    }
}
