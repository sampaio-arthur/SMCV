using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Contacts;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.Contacts.Queries.GetContactsByCampaign;

public class GetContactsByCampaignQueryHandler : IRequestHandler<GetContactsByCampaignQuery, IEnumerable<ContactResponse>>
{
    private readonly IContactRepository _contactRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly IMapper _mapper;

    public GetContactsByCampaignQueryHandler(
        IContactRepository contactRepository,
        ICampaignRepository campaignRepository,
        IMapper mapper)
    {
        _contactRepository = contactRepository;
        _campaignRepository = campaignRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContactResponse>> Handle(GetContactsByCampaignQuery request, CancellationToken cancellationToken)
    {
        if (!await _campaignRepository.ExistsAsync(request.CampaignId))
            throw new NotFoundException("Campaign", request.CampaignId);

        var contacts = await _contactRepository.GetByCampaignIdAsync(request.CampaignId);

        return _mapper.Map<IEnumerable<ContactResponse>>(contacts);
    }
}
