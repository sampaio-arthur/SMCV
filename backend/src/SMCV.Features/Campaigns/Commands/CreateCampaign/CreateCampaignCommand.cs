using MediatR;
using SMCV.Application.DTOs.Campaigns;

namespace SMCV.Features.Campaigns.Commands.CreateCampaign;

public record CreateCampaignCommand(
    Guid UserId,
    string Name,
    string Niche,
    string Region,
    string EmailSubject,
    string EmailBody
) : IRequest<CampaignResponse>;
