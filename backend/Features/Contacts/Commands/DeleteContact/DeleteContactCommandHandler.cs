using MediatR;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.Contacts.Commands.DeleteContact;

public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand>
{
    private readonly IContactRepository _contactRepository;

    public DeleteContactCommandHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Contact", request.Id);

        await _contactRepository.DeleteAsync(contact);
    }
}
