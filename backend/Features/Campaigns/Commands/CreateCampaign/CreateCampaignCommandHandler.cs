using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Campaigns;
using SMCV.Application.Interfaces;
using SMCV.Domain.Entities;
using SMCV.Domain.Enums;

namespace SMCV.Features.Campaigns.Commands.CreateCampaign;

public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, CampaignResponse>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IMapper _mapper;

    public CreateCampaignCommandHandler(ICampaignRepository campaignRepository, IMapper mapper)
    {
        _campaignRepository = campaignRepository;
        _mapper = mapper;
    }

    public async Task<CampaignResponse> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = new Campaign
        {
            Niche = request.Niche,
            Region = request.Region,
            ResumeFileName = request.ResumeFileName,
            ResumeFilePath = request.ResumeFilePath,
            EmailSubject = request.EmailSubject,
            EmailBody = request.EmailBody,
            Status = CampaignStatus.Draft
        };

        await _campaignRepository.AddAsync(campaign);

        return _mapper.Map<CampaignResponse>(campaign);
    }
}
