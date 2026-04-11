using MediatR;
using SMCV.Application.DTOs.Contacts;

namespace SMCV.Features.Contacts.Commands.CreateContact;

public record CreateContactCommand(
    Guid CampaignId,
    string CompanyName,
    string Email
) : IRequest<ContactResponse>;
