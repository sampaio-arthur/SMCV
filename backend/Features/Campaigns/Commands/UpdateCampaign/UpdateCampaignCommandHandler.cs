using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Campaigns;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;
using SMCV.Domain.Enums;

namespace SMCV.Features.Campaigns.Commands.UpdateCampaign;

public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, CampaignResponse>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IMapper _mapper;

    public UpdateCampaignCommandHandler(ICampaignRepository campaignRepository, IMapper mapper)
    {
        _campaignRepository = campaignRepository;
        _mapper = mapper;
    }

    public async Task<CampaignResponse> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _campaignRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Campaign", request.Id);

        if (campaign.Status != CampaignStatus.Draft)
            throw new InvalidOperationException("Apenas campanhas em Draft podem ser editadas.");

        campaign.EmailSubject = request.EmailSubject;
        campaign.EmailBody = request.EmailBody;

        await _campaignRepository.UpdateAsync(campaign);

        return _mapper.Map<CampaignResponse>(campaign);
    }
}
