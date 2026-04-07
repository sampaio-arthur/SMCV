using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Contacts;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.Contacts.Queries.GetContactById;

public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, ContactResponse>
{
    private readonly IContactRepository _contactRepository;
    private readonly IMapper _mapper;

    public GetContactByIdQueryHandler(IContactRepository contactRepository, IMapper mapper)
    {
        _contactRepository = contactRepository;
        _mapper = mapper;
    }

    public async Task<ContactResponse> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdWithEmailLogAsync(request.Id)
            ?? throw new NotFoundException("Contact", request.Id);

        return _mapper.Map<ContactResponse>(contact);
    }
}
