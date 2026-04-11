using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMCV.Application.DTOs;
using SMCV.Application.DTOs.Contacts;
using SMCV.Features.Contacts.Commands.CreateContact;
using SMCV.Features.Contacts.Commands.DeleteContact;
using SMCV.Features.Contacts.Commands.UpdateContact;
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
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetContactByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("campaign/{campaignId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ContactResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCampaign(Guid campaignId)
    {
        var result = await _mediator.Send(new GetContactsByCampaignQuery(campaignId));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateContactRequest request)
    {
        var command = new CreateContactCommand(
            request.CampaignId,
            request.CompanyName,
            request.Email);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactRequest request)
    {
        var command = new UpdateContactCommand(id, request.CompanyName, request.Email);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteContactCommand(id));
        return NoContent();
    }

}
