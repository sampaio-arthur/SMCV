using MediatR;

namespace SMCV.Features.Campaigns.Commands.ExportContactsCsv;

public record ExportContactsCsvCommand(Guid CampaignId) : IRequest<byte[]>;
