using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Contacts;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;
using SMCV.Domain.Entities;

namespace SMCV.Features.Contacts.Commands.CreateContact;

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactResponse>
{
    private readonly IContactRepository _contactRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly IMapper _mapper;

    public CreateContactCommandHandler(
        IContactRepository contactRepository,
        ICampaignRepository campaignRepository,
        IMapper mapper)
    {
        _contactRepository = contactRepository;
        _campaignRepository = campaignRepository;
        _mapper = mapper;
    }

    public async Task<ContactResponse> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        if (!await _campaignRepository.ExistsAsync(request.CampaignId))
            throw new NotFoundException("Campaign", request.CampaignId);

        var existing = await _contactRepository.GetByEmailAsync(request.Email, request.CampaignId);
        if (existing is not null)
            throw new InvalidOperationException("E-mail já cadastrado nesta campanha.");

        var contact = new Contact
        {
            CompanyName = request.CompanyName,
            Email = request.Email,
            Domain = request.Domain,
            ContactName = request.ContactName,
            Position = request.Position,
            Source = "manual",
            CampaignId = request.CampaignId
        };

        await _contactRepository.AddAsync(contact);

        return _mapper.Map<ContactResponse>(contact);
    }
}
