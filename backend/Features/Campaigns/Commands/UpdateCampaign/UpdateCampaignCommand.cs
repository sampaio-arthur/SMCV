using MediatR;
using SMCV.Application.DTOs.Campaigns;

namespace SMCV.Features.Campaigns.Commands.UpdateCampaign;

public record UpdateCampaignCommand(
    Guid Id,
    string Name,
    string EmailSubject,
    string EmailBody
) : IRequest<CampaignResponse>;
