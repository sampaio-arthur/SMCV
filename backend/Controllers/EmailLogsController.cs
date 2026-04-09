using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMCV.Application.DTOs;
using SMCV.Application.DTOs.EmailLogs;
using SMCV.Features.EmailLogs.Queries.GetEmailLogByContact;
using SMCV.Features.EmailLogs.Queries.GetEmailLogsByCampaign;

namespace SMCV.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmailLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("contact/{contactId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<EmailLogResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByContact(Guid contactId)
    {
        var result = await _mediator.Send(new GetEmailLogByContactQuery(contactId));
        return Ok(result);
    }

    [HttpGet("campaign/{campaignId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<EmailLogResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCampaign(Guid campaignId)
    {
        var result = await _mediator.Send(new GetEmailLogsByCampaignQuery(campaignId));
        return Ok(result);
    }
}
