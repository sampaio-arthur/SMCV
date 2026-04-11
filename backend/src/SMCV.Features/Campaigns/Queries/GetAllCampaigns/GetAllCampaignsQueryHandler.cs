using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Campaigns;
using SMCV.Application.Interfaces;

namespace SMCV.Features.Campaigns.Queries.GetAllCampaigns;

public class GetAllCampaignsQueryHandler : IRequestHandler<GetAllCampaignsQuery, IEnumerable<CampaignResponse>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IMapper _mapper;

    public GetAllCampaignsQueryHandler(ICampaignRepository campaignRepository, IMapper mapper)
    {
        _campaignRepository = campaignRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CampaignResponse>> Handle(GetAllCampaignsQuery request, CancellationToken cancellationToken)
    {
        var campaigns = await _campaignRepository.GetAllWithContactsAsync();

        return _mapper.Map<IEnumerable<CampaignResponse>>(campaigns);
    }
}
