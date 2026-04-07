using MediatR;
using SMCV.Application.DTOs.Contacts;

namespace SMCV.Features.Contacts.Commands.CreateContact;

public record CreateContactCommand(
    string CompanyName,
    string Email,
    string Domain,
    string? ContactName,
    string? Position,
    Guid CampaignId
) : IRequest<ContactResponse>;
