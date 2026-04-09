using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Contacts;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.Contacts.Commands.UpdateContact;

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, ContactResponse>
{
    private readonly IContactRepository _contactRepository;
    private readonly IMapper _mapper;

    public UpdateContactCommandHandler(IContactRepository contactRepository, IMapper mapper)
    {
        _contactRepository = contactRepository;
        _mapper = mapper;
    }

    public async Task<ContactResponse> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Contact", request.Id);

        contact.CompanyName = request.CompanyName;
        contact.Email = request.Email;

        await _contactRepository.UpdateAsync(contact);

        return _mapper.Map<ContactResponse>(contact);
    }
}
