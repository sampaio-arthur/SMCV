using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Campaigns;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.Campaigns.Queries.GetCampaignById;

public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, CampaignDetailResponse>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IMapper _mapper;

    public GetCampaignByIdQueryHandler(ICampaignRepository campaignRepository, IMapper mapper)
    {
        _campaignRepository = campaignRepository;
        _mapper = mapper;
    }

    public async Task<CampaignDetailResponse> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
    {
        var campaign = await _campaignRepository.GetByIdWithContactsAsync(request.Id)
            ?? throw new NotFoundException("Campaign", request.Id);

        return _mapper.Map<CampaignDetailResponse>(campaign);
    }
}
