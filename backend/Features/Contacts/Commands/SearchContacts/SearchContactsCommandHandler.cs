using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Contacts;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;
using SMCV.Domain.Entities;

namespace SMCV.Features.Contacts.Commands.SearchContacts;

public class SearchContactsCommandHandler : IRequestHandler<SearchContactsCommand, SearchContactsResponse>
{
    private readonly IContactRepository _contactRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly IHunterService _hunterService;
    private readonly IMapper _mapper;

    public SearchContactsCommandHandler(
        IContactRepository contactRepository,
        ICampaignRepository campaignRepository,
        IHunterService hunterService,
        IMapper mapper)
    {
        _contactRepository = contactRepository;
        _campaignRepository = campaignRepository;
        _hunterService = hunterService;
        _mapper = mapper;
    }

    public async Task<SearchContactsResponse> Handle(SearchContactsCommand request, CancellationToken cancellationToken)
    {
        if (!await _campaignRepository.ExistsAsync(request.CampaignId))
            throw new NotFoundException("Campaign", request.CampaignId);

        var results = await _hunterService.SearchContactsAsync(request.Niche, request.Region, request.Limit);

        var newContacts = new List<Contact>();

        foreach (var result in results)
        {
            var existing = await _contactRepository.GetByEmailAsync(result.Email, request.CampaignId);
            if (existing is not null)
                continue;

            var contact = new Contact
            {
                CompanyName = result.CompanyName,
                Email = result.Email,
                CampaignId = request.CampaignId
            };

            await _contactRepository.AddAsync(contact);
            newContacts.Add(contact);
        }

        var contactResponses = _mapper.Map<IEnumerable<ContactResponse>>(newContacts);

        return new SearchContactsResponse(newContacts.Count, contactResponses);
    }
}
