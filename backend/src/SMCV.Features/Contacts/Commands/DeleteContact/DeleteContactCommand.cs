using MediatR;

namespace SMCV.Features.Contacts.Commands.DeleteContact;

public record DeleteContactCommand(Guid Id) : IRequest;
