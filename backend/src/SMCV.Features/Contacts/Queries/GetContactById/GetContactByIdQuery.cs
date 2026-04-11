using MediatR;
using SMCV.Application.DTOs.Contacts;

namespace SMCV.Features.Contacts.Queries.GetContactById;

public record GetContactByIdQuery(Guid Id) : IRequest<ContactResponse>;
