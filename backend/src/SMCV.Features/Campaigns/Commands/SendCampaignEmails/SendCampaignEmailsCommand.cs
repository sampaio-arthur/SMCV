using MediatR;

namespace SMCV.Features.Campaigns.Commands.SendCampaignEmails;

public record SendCampaignEmailsCommand(Guid CampaignId) : IRequest<int>;
