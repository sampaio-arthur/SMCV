using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMCV.Application.DTOs.Contacts;
using SMCV.Features.Contacts.Commands.CreateContact;
using SMCV.Features.Contacts.Commands.DeleteContact;
using SMCV.Features.Contacts.Commands.SearchContacts;
using SMCV.Features.Contacts.Queries.GetContactById;
using SMCV.Features.Contacts.Queries.GetContactsByCampaign;

namespace SMCV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetContactByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("campaign/{campaignId:guid}")]
    public async Task<IActionResult> GetByCampaign(Guid campaignId)
    {
        var result = await _mediator.Send(new GetContactsByCampaignQuery(campaignId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactRequest request)
    {
        var command = new CreateContactCommand(
            request.CompanyName,
            request.Email,
            request.Domain,
            request.ContactName,
            request.Position,
            request.CampaignId);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteContactCommand(id));
        return NoContent();
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchContactsRequest request)
    {
        var command = new SearchContactsCommand(
            request.CampaignId,
            request.Niche,
            request.Region,
            request.Limit);

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
