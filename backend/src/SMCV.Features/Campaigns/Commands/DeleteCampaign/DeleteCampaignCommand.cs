using MediatR;

namespace SMCV.Features.Campaigns.Commands.DeleteCampaign;

public record DeleteCampaignCommand(Guid Id) : IRequest<bool>;
