using MediatR;
using SMCV.Application.DTOs.Contacts;

namespace SMCV.Features.Contacts.Commands.UpdateContact;

public record UpdateContactCommand(
    Guid Id,
    string CompanyName,
    string Email
) : IRequest<ContactResponse>;
