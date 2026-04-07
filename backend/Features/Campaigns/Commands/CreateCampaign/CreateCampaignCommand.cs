using MediatR;
using SMCV.Application.DTOs.Campaigns;

namespace SMCV.Features.Campaigns.Commands.CreateCampaign;

public record CreateCampaignCommand(
    string Niche,
    string Region,
    string ResumeFileName,
    string ResumeFilePath,
    string EmailSubject,
    string EmailBody
) : IRequest<CampaignResponse>;
